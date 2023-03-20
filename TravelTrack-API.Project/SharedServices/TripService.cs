using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Web.Http;
using TravelTrack_API.DbContexts;
using TravelTrack_API.Domain;
using TravelTrack_API.Versions.v1.DtoModels;
using v2 = TravelTrack_API.Versions.v2.DtoModels;
using TravelTrack_API.SharedServices.BlobManagement;

namespace TravelTrack_API.SharedServices;

public class TripService : ITripService
{
    private readonly TravelTrackContext _ctx;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    private readonly IUserService _userService;
    public TripService(TravelTrackContext ctx, IMapper mapper, IBlobService blobService, IUserService userService)
    {
        _ctx = ctx;
        _mapper = mapper;
        _blobService = blobService;
        _userService = userService;
    }


    /* * * * * * * * * * * * * * * * * *
     * Version 1 Methods
     * * * * * * * * * * * * * * * * * */
    
    public async Task<List<TripDto>> GetAllAsync()
    {
        List<Trip> trips = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .ToListAsync();

        List<TripDto> tripDTOs = _mapper.Map<List<TripDto>>(trips);
        return tripDTOs;
    }

    public async Task<TripDto> GetAsync(long id)
    {
        var trip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .IgnoreAutoIncludes()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {id}",
                    "Trip Id Not Found"
                )
            );
        }

        TripDto tripDTO = _mapper.Map<TripDto>(trip);
        return tripDTO;
    }

    public async Task<TripDto> AddAsync(TripDto trip)
    {
        TripNullCheck(trip);

        Trip tripEntity = new()
        {
            Title = trip.Title,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Details = trip.Details,
            ImgURL = trip.ImgURL,
        };

        foreach (DestinationDto destination in trip.Destinations)
        {
            var existingDestination = await _ctx.Destinations.FindAsync(destination.Id);

            // if destination does not exist in DB yet, add it to the DB
            if (existingDestination == null)
            {
                await _ctx.Destinations.AddAsync(new Destination
                {
                    Id = destination.Id,
                    City = destination.City,
                    Region = destination.Region,
                    Country = destination.Country,
                });
            }

            // add destination to trip
            tripEntity.Destinations.Add(new TripDestination
            {
                TripId = trip.Id,
                DestinationId = destination.Id,
            });
        }


        foreach (TripUserDto member in trip.Members)
        {
            // User must exist (already fully validated by frontend to DB)
            var existingUser = await _ctx.Users.FindAsync(member.Username);

            if (existingUser is not null)
            {
                // add member to trip
                tripEntity.Members.Add(new TripUser
                {
                    TripId = trip.Id,
                    Username = existingUser.Username,
                });
            }
            else
            {
                throw new HttpResponseException( // 400
                    ResponseMessage(
                        HttpStatusCode.BadRequest,
                        $"No User with Username = {member.Username}",
                        "Bad Request: Trip Member Does Not Exist"
                    )
                );
            }
        }


        await _ctx.Trips.AddAsync(tripEntity);
        await _ctx.SaveChangesAsync();

        trip.Id = tripEntity.Id;

        return trip;
    }

    public async Task<TripDto> UpdateAsync(long id, TripDto trip)
    {
        TripNullCheck(trip);

        if (id != trip.Id)
        {
            throw new HttpResponseException( // 400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"Id = {id} and provided Trip Id = {trip.Id} do not match",
                    "Bad Request: Id Mismatch"
                )
            );
        }

        var existingTrip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTrip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {id}",
                    "Trip Id Not Found"
                )
            );
        }

        existingTrip.Title = trip.Title;
        existingTrip.StartDate = trip.StartDate;
        existingTrip.EndDate = trip.EndDate;
        existingTrip.Details = trip.Details;
        existingTrip.ImgURL = trip.ImgURL;

        _ctx.TripDestinations.RemoveRange(existingTrip.Destinations);
        _ctx.TripMembers.RemoveRange(existingTrip.Members);
        _ctx.ToDo.RemoveRange(existingTrip.ToDo);
        _ctx.Photos.RemoveRange(existingTrip.Photos);

        foreach (DestinationDto destination in trip.Destinations)
        {
            var existingDestination = await _ctx.Destinations.FindAsync(destination.Id);

            // if destination does not exist in DB yet, add it to the DB
            if (existingDestination == null)
            {
                await _ctx.Destinations.AddAsync(new Destination
                {
                    Id = destination.Id,
                    City = destination.City,
                    Region = destination.Region,
                    Country = destination.Country,
                });
            }

            // add destination to trip
            existingTrip.Destinations.Add(new TripDestination
            {
                TripId = trip.Id,
                DestinationId = destination.Id,
            });
        }


        foreach (TripUserDto member in trip.Members)
        {
            // User must exist (already fully validated by frontend to DB)
            var existingUser = await _ctx.Users.FindAsync(member.Username);

            if (existingUser is not null)
            {
                // add member to trip
                existingTrip.Members.Add(new TripUser
                {
                    TripId = trip.Id,
                    Username = existingUser.Username,
                });
            }
            else
            {
                throw new HttpResponseException( //400
                    ResponseMessage(
                        HttpStatusCode.BadRequest,
                        $"No User with Username = {member.Username}",
                        "Bad Request: Trip Member Does Not Exist"
                    )
                );
            }
        }


        foreach (ToDoDto task in trip.ToDo)
        {
            existingTrip.ToDo.Add(new ToDo
            {
                Task = task.Task,
                Complete = task.Complete,
                TripId = trip.Id,
            });
        }

        foreach (PhotoDto photo in trip.Photos)
        {
            existingTrip.Photos.Add(new Photo
            {
                FileName = photo.FileName,
                FileType = photo.FileType,
                Path = photo.Path,
                AddedByUser = photo.AddedByUser,
                Alt = photo.Alt,
                TripId = trip.Id
            });
        }

        await _ctx.SaveChangesAsync();

        // EF auto updates autogenerated Ids as object properties,
        // so --> update generated Ids for returned trip DTO object
        if (trip.ToDo is not null && trip.ToDo.Count > 0)
        {
            var index = 0;
            foreach (ToDo task in existingTrip.ToDo)
            {
                trip.ToDo[index].Id = task.Id;
                index++;
            }
        }

        // maintain original GUIDs after SaveChanges()
        if (trip.Photos is not null && trip.Photos.Count > 0)
        {
            var index = 0;
            foreach (Photo photo in existingTrip.Photos)
            {
                trip.Photos[index].Id = photo.Id;
                index++;
            }
        }


        return trip;
    }

    public async Task<TripDto> AddPhotoToTripAsync(PhotoDto photo, IFormFile file, long tripId)
    {
        // null check photo
        photoNullCheck(photo);

        if (file is not null)
        {
            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                throw new HttpResponseException( // 400
                    ResponseMessage(
                        HttpStatusCode.BadRequest,
                        $"Invalid ContentType: File must be image/jpeg or image/png",
                        "Bad Request: Invalid File Type"
                    )
                );
            }
        }
        else
        {
            throw new HttpResponseException( // 400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"Null File: There must be a file provided as form data.",
                    "Bad Request: Null File"
                )
            );
        }

        // check that Trip Ids match
        if (photo.TripId != tripId)
        {
            throw new HttpResponseException( // 400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"Photo's TripId = {photo.TripId} and route's Trip Id = {tripId} do not match",
                    "Bad Request: Id Mismatch"
                )
            );
        }

        // get pre-existing trip
        var existingTrip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefaultAsync(t => t.Id == tripId);

        // check if trip exists
        if (existingTrip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {tripId}",
                    "Trip Not Found"
                )
            );
        }

        // if photo filename exists in blob storage already it will be replaced
        // so -> prevent duplicate file names from entering database bc they all refer to the same newest photo (front end will have validation)
        foreach (Photo p in existingTrip.Photos)
        {
            if (photo.FileName == p.FileName)
            {
                throw new HttpResponseException( // 409
                    ResponseMessage(
                        HttpStatusCode.Conflict,
                        $"Duplicate File: Photo FileName = {photo.FileName} already exists for this trip",
                        $"'{photo.Alt}' already exists for this trip."
                    )
                );
            }
        }

        // map to domain model and add pre-existing trip as navigation property
        Photo photoEntity = _mapper.Map<Photo>(photo);
        photoEntity.Trip = existingTrip;
        photoEntity.Id = Guid.NewGuid();

        // upload to blob storage and assign path
        photoEntity.Path = await _blobService.UploadPhotoToStorageAsync(file);

        await _ctx.Photos.AddAsync(photoEntity);
        await _ctx.SaveChangesAsync();

        // map back a response for trip
        TripDto tripDTO = _mapper.Map<TripDto>(existingTrip);
        return tripDTO;
    }

    public async Task<TripDto> RemovePhotosFromTripAsync(List<PhotoDto> photosToRemove, long tripId)
    {
        // null check collection of photos
        if (photosToRemove is null || photosToRemove.Count < 1)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Photos cannot be null",
                    "Bad Request: Null Photos"
                )
            );
        }

        // validation for each photo
        foreach (PhotoDto p in photosToRemove)
        {
            // null check each photo
            photoNullCheck(p);

            // check that Trip Ids match
            if (p.TripId != tripId)
            {
                throw new HttpResponseException( // 400
                    ResponseMessage(
                        HttpStatusCode.BadRequest,
                        $"Photo {p.FileName}'s  TripId = {p.TripId} and route's Trip Id = {tripId} do not match",
                        "Bad Request: Id Mismatch"
                    )
                );
            }
        }

        // get pre-existing trip
        var existingTrip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefaultAsync(t => t.Id == tripId);

        // check if trip exists
        if (existingTrip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {tripId}",
                    "Trip Not Found"
                )
            );
        }

        // remove each photo from the DB and it's correlated file form blob storage
        foreach (PhotoDto photo in photosToRemove)
        {
            Photo? photoEntity = await _ctx.Photos.FirstOrDefaultAsync(p => p.Id == photo.Id);
            _ctx.Photos.Remove(photoEntity!);
            await _blobService.DeleteAsync(photo.FileName);
        }

        await _ctx.SaveChangesAsync();
        TripDto trip = _mapper.Map<TripDto>(existingTrip);

        return trip;
    }


    // ------- private methods -------

    private void TripNullCheck(TripDto trip)
    {
        if (trip is null)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Trip cannot be null",
                    "Bad Request: Null Trip"
                )
            );
        }
        if (trip.Destinations is null || trip.Destinations.Count < 1)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Trip Destinations cannot be null",
                    "Bad Request: Null Trip.Destinations"
                )
            );
        }
        if (trip.Members is null || trip.Members.Count < 1)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Trip Members cannot be null",
                    "Bad Request: Null Trip.Members"
                )
            );
        }
    }



    /* * * * * * * * * * * * * * * * * *
     * Version 2 Methods
     * * * * * * * * * * * * * * * * * */

    public async Task<List<v2.TripDto>> GetTripsByUserIdAsync(string userId)
    {
        // validate user exists
        await B2CUserExistsAsync(userId);

        List<Trip> trips = await _ctx.Trips
            .Where(t => t.B2CMembers.Any(m => m.UserId == userId))
            .Include("Destinations.Destination")
            .Include("B2CMembers.B2CUser")
            .Include("ToDo")
            .Include("Photos")
            .ToListAsync();

        // create method to map returned b2cuser from microsoft graph to trip.members
        List<v2.TripDto> tripsB2CDTOs = _mapper.Map<List<v2.TripDto>>(trips);


        // no trips exist for this user
        if (trips is null)
        {
            throw new HttpResponseException( // 400
                    ResponseMessage(
                        HttpStatusCode.NoContent,
                        $"No Trips exist for user with Id = {userId}",
                        $"No Trips exist for user with Id = {userId}"
                    )
                );
        }

        // loop through each trip --> for each member id in members --> get user info from microsoft graph -->
        // 'manually' map result to member of current looped trip --> return completed trip list
        tripsB2CDTOs = await setMembersForMultipleTrips(tripsB2CDTOs);

        return tripsB2CDTOs;
    }

    public async Task<v2.TripDto> GetTripB2CAsync(long id)
    {
        var trip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("B2CMembers.B2CUser")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {id}",
                    "Trip Id Not Found"
                )
            );
        }

        v2.TripDto tripDto = _mapper.Map<v2.TripDto>(trip);

        return await setTripMembersFromADB2C(tripDto);
    }

    public async Task<v2.TripDto> AddAsync(v2.TripDto trip)
    {
        TripB2CNullCheck(trip);

        Trip tripEntity = new()
        {
            Title = trip.Title,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Details = trip.Details,
            ImgURL = trip.ImgURL,
        };

        foreach (DestinationDto destination in trip.Destinations)
        {
            var existingDestination = await _ctx.Destinations.FindAsync(destination.Id);

            // if destination does not exist in DB yet, add it to the DB
            if (existingDestination == null)
            {
                await _ctx.Destinations.AddAsync(new Destination
                {
                    Id = destination.Id,
                    City = destination.City,
                    Region = destination.Region,
                    Country = destination.Country,
                });
            }

            // add destination to trip
            tripEntity.Destinations.Add(new TripDestination
            {
                TripId = trip.Id,
                DestinationId = destination.Id,
            });
        }

        // B2C members
        foreach (v2.UserDto member in trip.Members)
        {
            // B2C User must exist
            // throws exception if not found
            await B2CUserExistsAsync(member.Id);

            // add member to trip
            tripEntity.B2CMembers.Add(new TripB2CUser
            {
                TripId = trip.Id,
                UserId = member.Id
            });
        }

        await _ctx.Trips.AddAsync(tripEntity);
        await _ctx.SaveChangesAsync();

        trip.Id = tripEntity.Id;

        return trip;
    }

    public async Task<v2.TripDto> UpdateAsync(long id, v2.TripDto trip)
    {
        TripB2CNullCheck(trip);

        if (id != trip.Id)
        {
            throw new HttpResponseException( // 400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"Id = {id} and provided Trip Id = {trip.Id} do not match",
                    "Bad Request: Id Mismatch"
                )
            );
        }

        var existingTrip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("B2CMembers.B2CUser")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTrip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {id}",
                    "Trip Id Not Found"
                )
            );
        }

        existingTrip.Title = trip.Title;
        existingTrip.StartDate = trip.StartDate;
        existingTrip.EndDate = trip.EndDate;
        existingTrip.Details = trip.Details;
        existingTrip.ImgURL = trip.ImgURL;

        _ctx.TripDestinations.RemoveRange(existingTrip.Destinations);
        _ctx.TripB2CMembers.RemoveRange(existingTrip.B2CMembers);
        _ctx.ToDo.RemoveRange(existingTrip.ToDo);
        _ctx.Photos.RemoveRange(existingTrip.Photos);

        foreach (DestinationDto destination in trip.Destinations)
        {
            var existingDestination = await _ctx.Destinations.FindAsync(destination.Id);

            // if destination does not exist in DB yet, add it to the DB
            if (existingDestination == null)
            {
                await _ctx.Destinations.AddAsync(new Destination
                {
                    Id = destination.Id,
                    City = destination.City,
                    Region = destination.Region,
                    Country = destination.Country,
                });
            }

            // add destination to trip
            existingTrip.Destinations.Add(new TripDestination
            {
                TripId = trip.Id,
                DestinationId = destination.Id,
            });
        }


        foreach (v2.UserDto member in trip.Members)
        {
            // B2C User must exist
            // throws exception if not found
            await B2CUserExistsAsync(member.Id);

            // add member to trip
            existingTrip.B2CMembers.Add(new TripB2CUser
            {
                TripId = trip.Id,
                UserId = member.Id
            });
        }

        foreach (ToDoDto task in trip.ToDo)
        {
            existingTrip.ToDo.Add(new ToDo
            {
                Task = task.Task,
                Complete = task.Complete,
                TripId = trip.Id,
            });
        }

        foreach (PhotoDto photo in trip.Photos)
        {
            existingTrip.Photos.Add(new Photo
            {
                FileName = photo.FileName,
                FileType = photo.FileType,
                Path = photo.Path,
                AddedByUser = photo.AddedByUser,
                Alt = photo.Alt,
                TripId = trip.Id
            });
        }

        await _ctx.SaveChangesAsync();

        // EF auto updates autogenerated Ids as object properties,
        // so --> update generated Ids for returned trip DTO object
        if (trip.ToDo is not null && trip.ToDo.Count > 0)
        {
            var index = 0;
            foreach (ToDo task in existingTrip.ToDo)
            {
                trip.ToDo[index].Id = task.Id;
                index++;
            }
        }

        // maintain original GUIDs after SaveChanges()
        if (trip.Photos is not null && trip.Photos.Count > 0)
        {
            var index = 0;
            foreach (Photo photo in existingTrip.Photos)
            {
                trip.Photos[index].Id = photo.Id;
                index++;
            }
        }

        return trip;
    }

    public async Task<v2.TripDto> AddPhotoToB2CTripAsync(PhotoDto photo, IFormFile file, long tripId)
    {
        // null check photo
        photoNullCheck(photo);

        if (file is not null)
        {
            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                throw new HttpResponseException( // 400
                    ResponseMessage(
                        HttpStatusCode.BadRequest,
                        $"Invalid ContentType: File must be image/jpeg or image/png",
                        "Bad Request: Invalid File Type"
                    )
                );
            }
        }
        else
        {
            throw new HttpResponseException( // 400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"Null File: There must be a file provided as form data.",
                    "Bad Request: Null File"
                )
            );
        }

        // check that Trip Ids match
        if (photo.TripId != tripId)
        {
            throw new HttpResponseException( // 400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"Photo's TripId = {photo.TripId} and route's Trip Id = {tripId} do not match",
                    "Bad Request: Id Mismatch"
                )
            );
        }

        // get pre-existing trip
        var existingTrip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("B2CMembers.B2CUser")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefaultAsync(t => t.Id == tripId);

        // check if trip exists
        if (existingTrip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {tripId}",
                    "Trip Not Found"
                )
            );
        }

        // if photo filename exists in blob storage already it will be replaced
        // so -> prevent duplicate file names from entering database bc they all refer to the same newest photo (front end will have validation)
        foreach (Photo p in existingTrip.Photos)
        {
            if (photo.FileName == p.FileName)
            {
                throw new HttpResponseException( // 409
                    ResponseMessage(
                        HttpStatusCode.Conflict,
                        $"Duplicate File: Photo FileName = {photo.FileName} already exists for this trip",
                        $"'{photo.Alt}' already exists for this trip."
                    )
                );
            }
        }

        // map to domain model and add pre-existing trip as navigation property
        Photo photoEntity = _mapper.Map<Photo>(photo);
        photoEntity.Trip = existingTrip;
        photoEntity.Id = Guid.NewGuid();

        // upload to blob storage and assign path
        photoEntity.Path = await _blobService.UploadPhotoToStorageAsync(file);

        await _ctx.Photos.AddAsync(photoEntity);
        await _ctx.SaveChangesAsync();

        // map back a response for trip
        v2.TripDto tripDto = _mapper.Map<v2.TripDto>(existingTrip);

        // get user data for each memmber from AD B2C
        tripDto = await setTripMembersFromADB2C(tripDto);

        return tripDto;
    }

    public async Task<v2.TripDto> RemovePhotosFromB2CTripAsync(List<PhotoDto> photosToRemove, long tripId)
    {
        // null check collection of photos
        if (photosToRemove is null || photosToRemove.Count < 1)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Photos cannot be null",
                    "Bad Request: Null Photos"
                )
            );
        }

        // validation for each photo
        foreach (PhotoDto p in photosToRemove)
        {
            // null check each photo
            photoNullCheck(p);

            // check that Trip Ids match
            if (p.TripId != tripId)
            {
                throw new HttpResponseException( // 400
                    ResponseMessage(
                        HttpStatusCode.BadRequest,
                        $"Photo {p.FileName}'s  TripId = {p.TripId} and route's Trip Id = {tripId} do not match",
                        "Bad Request: Id Mismatch"
                    )
                );
            }
        }

        // get pre-existing trip
        var existingTrip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("B2CMembers.B2CUser")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefaultAsync(t => t.Id == tripId);

        // check if trip exists
        if (existingTrip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {tripId}",
                    "Trip Not Found"
                )
            );
        }

        // remove each photo from the DB and it's correlated file form blob storage
        foreach (PhotoDto photo in photosToRemove)
        {
            Photo? photoEntity = await _ctx.Photos.FirstOrDefaultAsync(p => p.Id == photo.Id);
            _ctx.Photos.Remove(photoEntity!);
            await _blobService.DeleteAsync(photo.FileName);
        }

        await _ctx.SaveChangesAsync();
        v2.TripDto trip = _mapper.Map<v2.TripDto>(existingTrip);

        trip = await setTripMembersFromADB2C(trip);

        return trip;
    }

    // ------- private methods -------

    private void TripB2CNullCheck(v2.TripDto trip)
    {
        if (trip is null)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Trip cannot be null",
                    "Bad Request: Null Trip"
                )
            );
        }
        if (trip.Destinations is null || trip.Destinations.Count < 1)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Trip Destinations cannot be null",
                    "Bad Request: Null Trip.Destinations"
                )
            );
        }
        if (trip.Members is null || trip.Members.Count < 1)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Trip Members cannot be null",
                    "Bad Request: Null Trip.Members"
                )
            );
        }
    }

    // iterates a trip's list of members and fills in their user data from AD B2C
    private async Task<v2.TripDto> setTripMembersFromADB2C(v2.TripDto tripToBeUpdated)
    {
        // loop through trip members
        foreach (v2.UserDto member in tripToBeUpdated.Members)
        {
            // fetch user data from AD B2C
            // use  each member id to request user data via Microsoft Graph API
            v2.UserDto memberAsB2CUser = await _userService.GetB2CUserByIdAsync(member.Id);

            // fill in empty properties with AD B2C user data
            member.Username = memberAsB2CUser.Username;
            member.FirstName = memberAsB2CUser.FirstName;
            member.LastName = memberAsB2CUser.LastName;
            member.DisplayName = memberAsB2CUser.DisplayName;
        }
        return tripToBeUpdated;
    }

    // iterates a list of trips (v2.TripDto w/ user data from AD B2C)
    private async Task<List<v2.TripDto>> setMembersForMultipleTrips(List<v2.TripDto> tripsToBeUpdated)
    {
        List<v2.TripDto> updatedTrips = new List<v2.TripDto>();

        // loop through every trip
        foreach (v2.TripDto trip in tripsToBeUpdated)
        {
            // for every member in the current iterated trip
            updatedTrips.Add(await setTripMembersFromADB2C(trip));
        }


        return updatedTrips;
    }

    // checks if User exists in AD B2C
    private async Task B2CUserExistsAsync(string userId)
    {
        try
        {
            v2.UserDto user = await _userService.GetB2CUserByIdAsync(userId);
        }
        catch (HttpResponseException e)
        {
            if (e.Response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    $"User with Id = {userId} does not exist. Ensure all AD B2C Members are valid before submitting.",
                    $"Bad Request: User with Id = {userId} does not exist. All Trip B2C Members should be valid."
                )
            );
            }
        }
    }



    /* * * * * * * * * * * * * * * * * *
     * Shared Methods
     * * * * * * * * * * * * * * * * * */

    public async Task DeleteAsync(long id)
    {
        var trip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("B2CMembers.B2CUser")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip is null)
        {
            throw new HttpResponseException( // 404
                ResponseMessage(
                    HttpStatusCode.NotFound,
                    $"No Trip with Id = {id}",
                    "Trip Id Not Found"
                )
            );
        }

        foreach (Photo p in trip.Photos)
        {
            await _blobService.DeleteAsync(p.FileName);
        }

        _ctx.Remove(trip);
        await _ctx.SaveChangesAsync();
    }
    
    // Handle http exceptions
    public HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new ResponseContent.JsonContent(content),
            ReasonPhrase = reasonPhrase
        };
    }

    // ------- private methods -------

    private void photoNullCheck(PhotoDto photo)
    {
        if (photo is null)
        {
            throw new HttpResponseException( //400
                ResponseMessage(
                    HttpStatusCode.BadRequest,
                    "Photo cannot be null",
                    "Bad Request: Null Photo"
                )
            );
        }
    }
}