using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public interface IMiniProjectRepository : IBaseRepository<MiniProjectEntity, MiniProject>
{

}
public class MiniProjectRepository(AppDbContext context) : BaseRepository<MiniProjectEntity, MiniProject>(context), IMiniProjectRepository
{
}