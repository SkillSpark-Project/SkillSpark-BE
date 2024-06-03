using Application.Repositories;

namespace Application
{
    public interface IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public ITagRepository TagRepository { get; }


        public Task<int> SaveChangeAsync();
    }
}
