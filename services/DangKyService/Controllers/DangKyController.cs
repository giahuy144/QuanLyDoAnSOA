using Microsoft.AspNetCore.Mvc;
using DangKyService.DTOs;
using DangKyService.Services;

namespace DangKyService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DangKyController : ControllerBase
{
    private readonly IDangKyService _dangKyService;

    public DangKyController(IDangKyService dangKyService)
    {
        _dangKyService = dangKyService;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ các lượt đăng ký
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DangKyResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var list = await _dangKyService.GetAllDangKysAsync();
        return Ok(list);
    }

    /// <summary>
    /// Lấy thông tin một lượt đăng ký theo mã đăng ký
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DangKyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _dangKyService.GetDangKyByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { Message = $"Không tìm thấy đăng ký với mã '{id}'." });
        }
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách đăng ký theo mã sinh viên (dùng cho SinhVienService và WebClient)
    /// </summary>
    [HttpGet("sinhvien/{maSV}")]
    [ProducesResponseType(typeof(IEnumerable<DangKyResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMaSV(string maSV)
    {
        var list = await _dangKyService.GetDangKysByMaSVAsync(maSV.Trim().ToUpper());
        return Ok(list);
    }

    /// <summary>
    /// Lấy danh sách đăng ký theo mã đề tài (dùng cho DeTaiService và WebClient)
    /// </summary>
    [HttpGet("detai/{maDT}")]
    [ProducesResponseType(typeof(IEnumerable<DangKyResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMaDT(string maDT)
    {
        var list = await _dangKyService.GetDangKysByMaDTAsync(maDT.Trim().ToUpper());
        return Ok(list);
    }

    /// <summary>
    /// Đăng ký đề tài tốt nghiệp cho sinh viên (gọi liên dịch vụ sang SinhVienService và DeTaiService)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DangKyResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Create([FromBody] CreateDangKyDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, statusCode, errorMessage, result) = await _dangKyService.CreateDangKyAsync(dto);
        if (!success)
        {
            return StatusCode(statusCode, new { Message = errorMessage });
        }

        return CreatedAtAction(nameof(GetById), new { id = result!.MaDK }, result);
    }

    /// <summary>
    /// Cập nhật trạng thái của bản ghi đăng ký
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDangKyDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage) = await _dangKyService.UpdateTrangThaiAsync(id, dto);
        if (!success)
        {
            return NotFound(new { Message = errorMessage });
        }

        return NoContent();
    }

    /// <summary>
    /// Hủy / Xóa một bản ghi đăng ký
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, errorMessage) = await _dangKyService.DeleteDangKyAsync(id);
        if (!success)
        {
            return NotFound(new { Message = errorMessage });
        }

        return NoContent();
    }
}
