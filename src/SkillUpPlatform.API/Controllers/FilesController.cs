using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SkillUpPlatform.Application.Features.Files.Commands;
using SkillUpPlatform.Application.Features.Files.Queries;
using SkillUpPlatform.Application.Common.Models;
using SharedDtos = SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// File management endpoints for all users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("👨‍🎓 Student - File Management")]
public class FilesController : BaseController
{
    public FilesController(IMediator mediator) : base(mediator)
    {
    }
    /// <summary>
    /// Upload a file
    /// </summary>
    /// <param name="file">File to upload</param>
    /// <param name="description">File description (optional)</param>
    /// <param name="category">File category (optional)</param>
    /// <returns>Uploaded file information</returns>
    /// <response code="200">File uploaded successfully</response>
    /// <response code="400">Invalid file or file too large</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(Result<FileUploadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? description = null, [FromForm] string? category = null)
    {
        var command = new UploadFileCommand
        {
            File = file,
            Description = description,
            Category = category
        };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Upload multiple files
    /// </summary>
    /// <param name="files">Files to upload</param>
    /// <param name="category">File category (optional)</param>
    /// <returns>List of uploaded file information</returns>
    /// <response code="200">Files uploaded successfully</response>
    /// <response code="400">Invalid files or files too large</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("upload-multiple")]
    [ProducesResponseType(typeof(Result<List<FileUploadDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadMultiple([FromForm] List<IFormFile> files, [FromForm] string? category = null)
    {
        var command = new UploadMultipleFilesCommand
        {
            Files = files,
            Category = category
        };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Download a file
    /// </summary>
    /// <param name="id">File ID</param>
    /// <returns>File download stream</returns>
    /// <response code="200">File downloaded successfully</response>
    /// <response code="404">File not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Download(int id)
    {
        var query = new GetFileDownloadQuery { FileId = id, UserId = GetCurrentUserId() };
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
            return HandleResult(result);
            
        var fileData = result.Data!;
        return File(fileData.FileContent, fileData.FileType, fileData.FileName);
    }

    /// <summary>
    /// Get user's files
    /// </summary>
    /// <param name="category">Filter by category (optional)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>List of user's files</returns>
    /// <response code="200">Files retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("my-files")]
    [ProducesResponseType(typeof(Result<PagedResult<FileInfoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyFiles([FromQuery] string? category = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetUserFilesQuery
        {
            UserId = GetCurrentUserId(),
            Category = category,
            Page = page,
            PageSize = pageSize
        };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get file details
    /// </summary>
    /// <param name="id">File ID</param>
    /// <returns>File details</returns>
    /// <response code="200">File details retrieved successfully</response>
    /// <response code="404">File not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet("{id}/details")]
    [ProducesResponseType(typeof(Result<SharedDtos.FileDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFileDetails(int id)
    {
        var query = new GetFileDetailsQuery { FileId = id, UserId = GetCurrentUserId() };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Delete a file
    /// </summary>
    /// <param name="id">File ID</param>
    /// <returns>Delete result</returns>
    /// <response code="200">File deleted successfully</response>
    /// <response code="404">File not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteFileCommand { FileId = id };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Update file information
    /// </summary>
    /// <param name="id">File ID</param>
    /// <param name="command">File update data</param>
    /// <returns>Update result</returns>
    /// <response code="200">File updated successfully</response>
    /// <response code="404">File not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFileCommand command)
    {
        command.FileId = id;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Share a file with other users
    /// </summary>
    /// <param name="id">File ID</param>
    /// <param name="command">Share settings</param>
    /// <returns>Share result</returns>
    /// <response code="200">File shared successfully</response>
    /// <response code="404">File not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPost("{id}/share")]
    [ProducesResponseType(typeof(Result<FileShareDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Share(int id, [FromBody] ShareFileCommand command)
    {
        command.FileId = id;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get file categories
    /// </summary>
    /// <returns>List of available file categories</returns>
    /// <response code="200">Categories retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(Result<List<FileCategoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategories()
    {
        var query = new GetFileCategoriesQuery();
        return HandleResult(await _mediator.Send(query));
    }
}
