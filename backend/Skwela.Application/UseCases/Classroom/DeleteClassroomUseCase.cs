using Skwela.Application.Interfaces;
using Skwela.Domain.Entities;

namespace Skwela.Application.UseCases.Classrooms;

public class DeleteClassroomUseCase
{

  private readonly IClassroomRepository _repository;

  public DeleteClassroomUseCase(IClassroomRepository repository) 
  {
    _repository = repository;
  }

  public async Task ExecuteRemoveClassroomAsync(Guid classId)
  {
    await _repository.RemoveClassroomAsync(classId);
  }

}