using AutoMapper;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using MediatR;

public abstract class BaseCreateCommandHandler<TCommand, TEntity, TDto> : IRequestHandler<TCommand, TDto>
    where TCommand : IRequest<TDto>
    where TEntity : class, new()
{
    protected readonly IRepository<TEntity> _repository;
    protected readonly IMapper _mapper;

    protected BaseCreateCommandHandler(IRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TDto> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<TEntity>(request);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<TDto>(result);
    }
}
