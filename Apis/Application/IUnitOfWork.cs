using Application.Repositories;

namespace Application
{
    public interface IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public ITagRepository TagRepository { get; }
        public ILearnerRepository LearnerRepository { get; }
        public IMentorRepository MentorRepository { get; }
        public IContentRepository ContentRepository { get; }
        public IRequirementRepository RequirementRepository { get; }
        public Task<int> SaveChangeAsync();
        public void BeginTransaction();
        public void BeginTransactionLocking();
        public Task CommitTransactionAsync();
        public void RollbackTransaction();
        public void ClearTrack();
    }
}
