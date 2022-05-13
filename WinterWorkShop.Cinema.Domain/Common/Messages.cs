namespace WinterWorkShop.Cinema.Domain.Common
{
    public static class Messages
    {
        #region Users

        #endregion

        #region Payments
        public const string PAYMENT_CREATION_ERROR = "Connection error, occured while creating new payment, please try again";
        #endregion

        #region Auditoriums
        public const string AUDITORIUM_GET_ALL_AUDITORIUMS_ERROR = "Error occured while getting all auditoriums, please try again.";
        public const string AUDITORIUM_PROPERTIE_NAME_NOT_VALID = "The auditorium Name cannot be longer than 50 characters.";
        public const string AUDITORIUM_PROPERTIE_SEATROWSNUMBER_NOT_VALID = "The auditorium number of seats rows must be between 1-20.";
        public const string AUDITORIUM_PROPERTIE_SEATNUMBER_NOT_VALID = "The auditorium number of seats number must be between 1-20.";
        public const string AUDITORIUM_CREATION_ERROR = "Error occured while creating new auditorium, please try again.";
        public const string AUDITORIUM_SEATS_CREATION_ERROR = "Error occured while creating seats for auditorium, please try again.";
        public const string AUDITORIUM_SAME_NAME = "Cannot create new auditorium, auditorium with same name alredy exist.";
        public const string AUDITORIUM_UNVALID_CINEMAID = "Cannot create new auditorium, auditorium with given cinemaId does not exist.";
        #endregion

        #region Cinemas
        public const string CINEMA_GET_ALL_CINEMAS_ERROR = "Error occured while getting all cinemas, please try again";
        public const string CINEMA_PROPERTY_NAME_NOT_VALID= "Cinema name cannot be longer than 50 characters.";
        public const string CINEMA_CREATION_ERROR = "Error occured while creating new cinema, please try again.";
        public const string CINEMA_DOES_NOT_EXIST_ERROR = "Cinema with inserted id does not exist.";
        #endregion

        #region Movies        
        public const string MOVIE_DOES_NOT_EXIST = "Movie does not exist.";
        public const string MOVIE_PROPERTIE_TITLE_NOT_VALID = "The movie title cannot be longer than 50 characters.";
        public const string MOVIE_PROPERTIE_YEAR_NOT_VALID = "The movie year must be between 1895-2100.";
        public const string MOVIE_PROPERTIE_RATING_NOT_VALID = "The movie rating must be between 1-10.";
        public const string MOVIE_CREATION_ERROR = "Error occured while creating new movie, please try again.";
        public const string MOVIE_GET_ALL_CURRENT_MOVIES_ERROR = "Error occured while getting current movies, please try again.";
        public const string MOVIE_GET_BY_ID = "Error occured while getting movie by Id, please try again.";
        public const string MOVIE_GET_ALL_MOVIES_ERROR = "Error occured while getting all movies, please try again.";
        public const string MOVIE_HAS_FUTURE_PROJECTIONS = "You can't deactivate movie with inserted id because this movie has projections in future";
        #endregion

        #region Projections
        public const string PROJECTION_GET_ALL_PROJECTIONS_ERROR = "Error occured while getting all projections, please try again.";
        public const string PROJECTION_CREATION_ERROR = "Error occured while creating new projection, please try again.";
        public const string PROJECTIONS_AT_SAME_TIME = "Cannot create new projection, there are projections at same time alredy.";
        public const string PROJECTION_IN_PAST = "Projection time cannot be in past.";
        public const string PROJECTION_EXISTS_ERROR = "Projection already exists.";
        public const string PROJECTION_DOESNT_EXIST_ERROR = "Projection doesn't exist.";
        #endregion

        #region Seats
        public const string SEAT_GET_ALL_SEATS_ERROR = "Error occured while getting all seats, please try again.";
        public const string SEAT_NUMBER_ERROR = "Seat number must be in range from 1 to 10";
        public const string SEAT_ROW_ERROR = "Row number must be in range from 1 to 10";
        public const string SEAT_CREATION_ERROR = "Error occured while adding new seat, please try again.";
        public const string SEAT_EXISTS_ERROR = "Seat already exists.";
        public const string SEAT_DOESNT_EXIST_ERROR = "Seat doesn't exist.";
        public const string SEAT_RESERVED_ERROR = "Seat already reserved.";
        public const string SEAT_RESERVATION_ERROR = "Error occured while trying to reserve a seat, please try again.";
        public const string SEATS_NOT_CONSECUTIVE_ERROR = "Seats are not consecutive.";
        public const string SEATS_ROW_ERROR = "You can't reserve seat in chosen row.";
        #endregion

        #region User
        public const string USER_NOT_FOUND = "User does not exist.";
        public const string USER_PROPERTIE_FIRSTNAME_NOT_VALID = "First name cannot be longer than 50 characters.";
        public const string USER_PROPERTIE_LASTNAME_VALID = "Last name cannot be longer than 50 characters.";
        public const string USER_PROPERTIE_USERNAME_NOT_VALID = "User name cannot be longer than 50 characters.";
        public const string USER_CREATION_ERROR = "Error occured while creating new user, please try again.";
        #endregion

        #region Tickets
        public const string TICKET_CREATION_ERROR = "Error occured while adding new ticket, please try again.";
        public const string SEAT_FOR_PROJECTION_ERROR = "Chosen seat for projection is already reserved";
        #endregion

        #region Tags
        public const string TAG_CREATION_ERROR = "Error occured while adding new tag, please try again.";
        public const string TAG_DOESNT_EXIST = "Tag with inserted id doesn't exist";
        #endregion
    }
}
