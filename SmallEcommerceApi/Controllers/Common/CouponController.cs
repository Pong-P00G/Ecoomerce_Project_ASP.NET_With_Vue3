using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallEcommerceApi.Models;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Controllers.Common
{
    [ApiController]
    [Route("api/coupons")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _service;

        public CouponController(ICouponService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var coupon = await _service.GetByCodeAsync(code);
            if (coupon == null) return NotFound();
            return Ok(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            var result = await _service.CreateAsync(coupon);
            return CreatedAtAction(nameof(GetByCode), new { code = result.CouponCode }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Coupon coupon)
        {
            var result = await _service.UpdateAsync(id, coupon);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }

}
