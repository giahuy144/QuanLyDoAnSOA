using Microsoft.AspNetCore.Mvc;
using DeTaiService.DTOs;
using DeTaiService.Services;

namespace DeTaiService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DeTaiController : ControllerBase
{
    private readonly IDeTaiService _deTaiService;

    public DeTaiController(IDeTaiService deTaiService)
    {
        _deTaiService = deTaiService;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ đề tài
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DeTaiResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var list = await _deTaiService.GetAllDeTaisAsync();
        return Ok(list);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một đề tài theo mã đề tài
    /// </summary>
    [HttpGet("{maDT}")]
    [ProducesResponseType(typeof(DeTaiResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string maDT)
    {
        var dt = await _deTaiService.GetDeTaiByIdAsync(maDT);
        if (dt == null)
        {
            return NotFound(new { Message = $"Không tìm thấy đề tài với mã '{maDT}'." });
        }
        return Ok(dt);
    }

    /// <summary>
    /// Thêm mới đề tài
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DeTaiResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDeTaiDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage, result) = await _deTaiService.CreateDeTaiAsync(dto);
        if (!success)
        {
            return Conflict(new { Message = errorMessage });
        }

        return CreatedAtAction(nameof(GetById), new { maDT = result!.MaDT }, result);
    }

    /// <summary>
    /// Cập nhật thông tin đề tài
    /// </summary>
    [HttpPut("{maDT}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string maDT, [FromBody] UpdateDeTaiDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage) = await _deTaiService.UpdateDeTaiAsync(maDT, dto);
        if (!success)
        {
            return NotFound(new { Message = errorMessage });
        }

        return NoContent();
    }

    /// <summary>
    /// Xóa đề tài theo mã đề tài
    /// </summary>
    [HttpDelete("{maDT}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(string maDT)
    {
        var (success, errorMessage, isConflict) = await _deTaiService.DeleteDeTaiAsync(maDT);
        if (!success)
        {
            if (isConflict)
            {
                return Conflict(new { Message = errorMessage });
            }
            return NotFound(new { Message = errorMessage });
        }

        return NoContent();
    }
}
