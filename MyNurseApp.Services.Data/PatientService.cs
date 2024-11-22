using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using System.Globalization;

namespace MyNurseApp.Services.Data
{
    public class PatientService
    {
        public async Task<bool> AddMovieAsync(AddMovieInputModel inputModel)
        {
            private readonly IRepository<PatientProfile,string> patientRepository;

            bool isReleaseDateValid = DateTime
                .TryParseExact(inputModel.ReleaseDate, ReleaseDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime releaseDate);
            if (!isReleaseDateValid)
            {
                return false;
            }

            Movie movie = new Movie();
            AutoMapperConfig.MapperInstance.Map(inputModel, movie);
            movie.ReleaseDate = releaseDate;

            await this.movieRepository.AddAsync(movie);

            return true;
        }
    }
}
