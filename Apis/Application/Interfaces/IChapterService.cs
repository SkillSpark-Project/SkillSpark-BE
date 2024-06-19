using Application.ViewModels.ChapterViewModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IChapterService
    {
        public Task<IList<Chapter>> GetChapters();

        public Task AddChapter(ChapterModel model, string userId);

        public Task UpdateChapter(Guid id, ChapterModel model, string userId);

        public Task DeleteChapter(Guid id, string userId);

    }
}
