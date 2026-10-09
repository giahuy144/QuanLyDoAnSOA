using Microsoft.AspNetCore.Mvc;
using SinhVienService.DTOs;
using SinhVienService.Services;

namespace SinhVienService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SinhVienController : ControllerBase
{
    private readonly ISinhVienService _sinhVienService;

    public SinhVienController(ISinhVienService sinhVienService)
    {
        _sinhVienService = sinhVienService;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ sinh viên
    /// </summary>
    /// <response code="200">Trả về danh sách sinh viên</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SinhVienResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var list = await _sinhVienService.GetAllSinhViensAsync();
        return Ok(list);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một sinh viên theo mã sinh viên
    /// </summary>
    /// <param name="maSV">Mã sinh viên</param>
    /// <response code="200">Tìm thấy sinh viên</response>
    /// <response code="404">Không tìm thấy sinh viên</response>
    [HttpGet("{maSV}")]
    [ProducesResponseType(typeof(SinhVienResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string maSV)
    {
        var sv = await _sinhVienService.GetSinhVienByIdAsync(maSV);
        if (sv == null)
        {
            return NotFound(new { Message = $"Không tìm thấy sinh viên với mã '{maSV}'." });
        }
        return Ok(sv);
    }

    /// <summary>
    /// Thêm mới sinh viên
    /// </summary>
    /// <param name="dto">Dữ liệu sinh viên cần tạo</param>
    /// <response code="201">Tạo thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="409">Mã sinh viên đã tồn tại</response>
    [HttpPost]
    [ProducesResponseType(typeof(SinhVienResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSinhVienDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage, result) = await _sinhVienService.CreateSinhVienAsync(dto);
        if (!success)
        {
            return Conflict(new { Message = errorMessage });
        }

        return CreatedAtAction(nameof(GetById), new { maSV = result!.MaSV }, result);
    }

    /// <summary>
    /// Cập nhật thông tin sinh viên
    /// </summary>
    /// <param name="maSV">Mã sinh viên</param>
    /// <param name="dto">Thông tin cập nhật</param>
    /// <response code="204">Cập nhật thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="404">Không tìm thấy sinh viên</response>
    [HttpPut("{maSV}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string maSV, [FromBody] UpdateSinhVienDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage) = await _sinhVienService.UpdateSinhVienAsync(maSV, dto);
        if (!success)
        {
            return NotFound(new { Message = errorMessage });
        }

        return NoContent();
    }

    /// <summary>
    /// Xóa sinh viên theo mã sinh viên
    /// </summary>
    /// <param name="maSV">Mã sinh viên</param>
    /// <response code="204">Xóa thành công</response>
    /// <response code="404">Không tìm thấy sinh viên</response>
    /// <response code="409">Sinh viên đang có đăng ký đồ án, không được phép xóa</response>
    [HttpDelete("{maSV}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(string maSV)
    {
        var (success, errorMessage, isConflict) = await _sinhVienService.DeleteSinhVienAsync(maSV);
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
