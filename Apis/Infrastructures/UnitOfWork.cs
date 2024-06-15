using Application;
using Application.Repositories;
using Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _dbContext;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMentorRepository _mentorRepository;
        private readonly ILearnerRepository _learnerRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IRequirementRepository _requirementRepository;
        private readonly ICourseTagRepository _courseTagRepository;
        private IDbContextTransaction _transaction;


        public UnitOfWork(AppDbContext dbContext, ICategoryRepository categoryRepository, ICourseRepository courseRepository,
            ITagRepository tagRepository, IMentorRepository mentorRepository, ILearnerRepository learnerRepository,
            IContentRepository contentRepository , IRequirementRepository requirementRepository, ICourseTagRepository courseTagRepository
         )
        {
            _dbContext = dbContext;
            _categoryRepository = categoryRepository;
            _courseRepository = courseRepository;
            _tagRepository = tagRepository;
            _mentorRepository = mentorRepository;
            _learnerRepository = learnerRepository;
            _contentRepository = contentRepository;
            _requirementRepository = requirementRepository;
            _courseTagRepository = courseTagRepository;
        }
        public ICategoryRepository CategoryRepository => _categoryRepository;
        public ICourseRepository CourseRepository => _courseRepository;
        public ITagRepository TagRepository => _tagRepository;
        public ILearnerRepository LearnerRepository => _learnerRepository;
        public IMentorRepository MentorRepository => _mentorRepository;

        public IContentRepository ContentRepository => _contentRepository;

        public IRequirementRepository RequirementRepository => _requirementRepository;

        public ICourseTagRepository CourseTagRepository => _courseTagRepository;

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void BeginTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public void BeginTransactionLocking()
        {
            _transaction = _dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        }
        public async Task CommitTransactionAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }
        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }
        public void ClearTrack()
        {
            _dbContext.ChangeTracker.Clear();
        }
    }
}
