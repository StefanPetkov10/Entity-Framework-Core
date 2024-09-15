using CinemaApp.Core.Contracts;
using CinemaApp.Infrastructure.Data.Common;
using CinemaApp.Infrastructure.Data.Models;

namespace CinemaApp.Core.Services
{
    public class MovieService : IMovieService
    {
        private readonly IRepository _repository;

        public MovieService(IRepository repository)
        {
            _repository = repository;
        }

        public IList<Movie> GetAllMovies()
        {
            return _repository.All<Movie>()
                .ToList();
        }

        public IQueryable<Movie> GetAllMovies(Func<Movie, bool> predicate)
        {
            return _repository.All<Movie>()
                .Where(predicate)
                .AsQueryable();
        }

        public IQueryable<Movie> GetAllMoviesPaged(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentException(nameof(pageNumber));
            }

            if (pageSize < 1)
            {
                throw new ArgumentException(nameof(pageSize));
            }
            return _repository.All<Movie>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

    }
}
