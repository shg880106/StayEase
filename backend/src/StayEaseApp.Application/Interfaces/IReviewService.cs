using StayEaseApp.Application.DTOs;
using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Interfaces;
public interface IReviewService
{
    Task<ReviewResponseDto?> GetReviewByIdAsync(Guid reviewId);
    Task<ReviewResponseDto> CreateReviewAsync(CreateReviewRequestDto reviewRequest);
}
