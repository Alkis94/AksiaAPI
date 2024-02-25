using AksiaAPI.Models.Business;
using AksiaAPI.Models.DTOs;
using AksiaAPI.Models.Entities;
using AksiaAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AksiaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid id)
        {
            var transaction = await _transactionService.Get(id);
            var dto = _mapper.Map<TransactionDto>(transaction);

            return  Ok(dto);
        }

        [HttpGet("GetPaged")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] Page page)
        {
            page ??= new Page();

            var transactions = await _transactionService.GetPagedAsync(page);
            var dtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);

            return Ok(dtos);
        }

        [HttpPost("ImportFromCSV")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<IActionResult> ImportFromCSV(IFormFile file)
        {
            if (Path.GetExtension(file.FileName) != ".csv")
            {
                throw new BusinessException("Only .csv files are accepted.");
            }

            var result = await _transactionService.ParseCSV(file);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] TransactionInsertDto transactionDto)
        {
            var transaction = _mapper.Map<TransactionInsertOrUpdate>(transactionDto);
            var id = await _transactionService.Insert(transaction);

            return Ok(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _transactionService.Delete(id);

            return Ok();
        }
    }
}
