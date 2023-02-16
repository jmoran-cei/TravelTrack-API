using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Web.Http;
using TravelTrack_API.DbContexts;
using TravelTrack_API.Domain;
using TravelTrack_API.DTO;
using TravelTrack_API.Services.BlobManagement;

namespace TravelTrack_API.Services;

public class TripService : ITripService
{
    private readonly TravelTrackContext _ctx;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    public TripService(TravelTrackContext ctx, IMapper mapper, IBlobService blobService)
    {
        _ctx = ctx;
        _mapper = mapper;
        _blobService = blobService;
    }

    public List<TripDto> GetAll()
    {
        List<Trip> trips = _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .ToList();

        List<TripDto> tripDTOs = _mapper.Map<List<TripDto>>(trips);
        return tripDTOs;
    }

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
    public TripDto Get(long id)
    {
        var trip = _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefault(t => t.Id == id);

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

    public async Task<TripDto> GetAsync(long id)
    {
        var trip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
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

        TripDto tripDTO = _mapper.Map<TripDto>(trip);
        return tripDTO;
    }

    public TripDto Add(TripDto trip)
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
            var existingDestination = _ctx.Destinations.Find(destination.Id);

            // if destination does not exist in DB yet, add it to the DB
            if (existingDestination == null)
            {
                _ctx.Destinations.Add(new Destination
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
            var existingUser = _ctx.Users.Find(member.Username);

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


        _ctx.Trips.Add(tripEntity);
        _ctx.SaveChanges();

        trip.Id = tripEntity.Id;

        return trip;
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

    public void Delete(long id)
    {
        var trip = _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefault(t => t.Id == id);

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
            _blobService.Delete(p.FileName);
        }

        _ctx.Remove(trip);
        _ctx.SaveChanges();
    }

    public async Task DeleteAsync(long id)
    {
        var trip = await _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
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

    public TripDto Update(long id, TripDto trip)
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

        var existingTrip = _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefault(t => t.Id == id);

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
        _ctx.TripUsers.RemoveRange(existingTrip.Members);
        _ctx.ToDo.RemoveRange(existingTrip.ToDo);
        _ctx.Photos.RemoveRange(existingTrip.Photos);

        foreach (DestinationDto destination in trip.Destinations)
        {
            var existingDestination = _ctx.Destinations.Find(destination.Id);

            // if destination does not exist in DB yet, add it to the DB
            if (existingDestination == null)
            {
                _ctx.Destinations.Add(new Destination
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
            var existingUser = _ctx.Users.Find(member.Username);

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

        _ctx.SaveChanges();

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
        _ctx.TripUsers.RemoveRange(existingTrip.Members);
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

    public TripDto AddPhotoToTrip(PhotoDto photo, IFormFile file, long tripId)
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
        var existingTrip = _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefault(t => t.Id == tripId);

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
        photoEntity.Path = _blobService.UploadPhotoToStorage(file!);

        _ctx.Photos.Add(photoEntity);
        _ctx.SaveChanges();

        // map back a response for trip
        TripDto tripDTO = _mapper.Map<TripDto>(existingTrip);
        return tripDTO;
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

    public TripDto RemovePhotosFromTrip(List<PhotoDto> photosToRemove, long tripId)
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
        var existingTrip = _ctx.Trips
            .Include("Destinations.Destination")
            .Include("Members.User")
            .Include("ToDo")
            .Include("Photos")
            .FirstOrDefault(t => t.Id == tripId);

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
            Photo photoEntity = _ctx.Photos.FirstOrDefault(p => p.Id == photo.Id)!;
            _ctx.Photos.Remove(photoEntity);
            _blobService.Delete(photo.FileName);
        }

        _ctx.SaveChanges();
        TripDto trip = _mapper.Map<TripDto>(existingTrip);

        return trip;
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
        if (trip.Members is null || trip.Destinations.Count < 1)
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

    // ------- public http exception method -------
    public HttpResponseMessage ResponseMessage(HttpStatusCode statusCode, string content, string reasonPhrase)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new ResponseContent.JsonContent(content),
            ReasonPhrase = reasonPhrase
        };
    }
}