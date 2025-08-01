﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TeachMate.Domain;
using TeachMate.Services;

namespace TeachMate.Api;
[Route("api/[controller]")]
[ApiController]
public class LearningModuleController : ControllerBase
{
    private readonly IHttpContextService _contextService;
    private readonly ILearningModuleService _learningModuleService;

    public LearningModuleController(ILearningModuleService learningModuleService, IHttpContextService contextService)
    {
        _learningModuleService = learningModuleService;
        _contextService = contextService;
    }

    /// <summary>
    /// Get All Created Modules
    /// </summary>
    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpGet("Tutor/GetAll")]
    public async Task<ActionResult<List<LearningModule>>> GetAllCreatedModules()
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.GetAllCreatedModules(user));
    }

    /// <summary>
    /// Get All Enrolled Modules
    /// </summary>
    [Authorize(Roles = CustomRoles.Learner)]
    [HttpGet("Learner/GetAll")]
    public async Task<ActionResult<List<LearningModule>>> GetAllEnrolledModules()
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.GetAllEnrolledModules(user));
    }

    // TODO: Add filter here to check if learning module created by or enrolled by current user
    /// <summary>
    /// Get LearningModule by Id
    /// </summary>
    [Authorize(Roles = CustomRoles.GeneralUser)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LearningModule?>> GetLearningModuleById(int id)
    {
        return Ok(await _learningModuleService.GetLearningModuleById(id));
    }

    ///// <summary>
    ///// Enroll Learning Module
    ///// </summary>
    [Authorize(Roles = CustomRoles.Learner)]
    [HttpGet("OutClass/{moduleId:int}")]
    public async Task<ActionResult<LearningModule>> outClass(int moduleId)
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.OutClass(user.Id, moduleId));
    }
    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpPost("KickLearner/")]
    public async Task<ActionResult<LearningModule>> KickLearner(KickLearnerDto dto)
    {
        return Ok(await _learningModuleService.KickLearner(dto));
    }

    /// <summary>
    /// Create Learning Module
    /// </summary>
    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpPost("Create")]
    public async Task<ActionResult<LearningModule>> CreateLearningModule(CreateLearningModuleDto dto)
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.CreateLearningModule(user, dto));
    }

    /// <summary>
    /// Get all received requests
    /// </summary>
    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpGet("Request/Tutor/GetAll")]
    public async Task<ActionResult<List<LearningModuleRequest>>> GetAllReceivedRequests()
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.GetAllReceivedRequests(user.Id));
    }

    /// <summary>
    /// Get all received requests from a learning module
    /// </summary>
    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpGet("Request/LearningModule/GetAll")]
    public async Task<ActionResult<List<LearningModuleRequest>>> GetAllReceivedRequests(int moduleId)
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.GetAllReceivedRequests(moduleId, user.Id));
    }

    /// <summary>
    /// Get all created requests
    /// </summary>
    [Authorize(Roles = CustomRoles.Learner)]
    [HttpGet("Request/Learner/GetAll")]
    public async Task<ActionResult<List<LearningModuleRequest>>> GetAllCreatedRequests()
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.GetAllCreatedRequests(user.Id));
    }

    // TODO: Add filter here to check if request created by or applied to current user
    /// <summary>
    /// Get Request by Id
    /// </summary>
    [Authorize(Roles = CustomRoles.GeneralUser)]
    [HttpGet("Request/{id:int}")]
    public async Task<ActionResult<LearningModuleRequest?>> GetRequestById(int id)
    {
        return Ok(await _learningModuleService.GetRequestById(id));
    }

    /// <summary>
    /// Create Learning Module Request
    /// </summary>
    [Authorize(Roles = CustomRoles.Learner)]
    [HttpPost("Request/Create")]
    public async Task<ActionResult<LearningModuleRequest>> CreateLearningModuleRequest(CreateLearningModuleRequestDto dto)
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.CreateLearningModuleRequest(user, dto));
    }

    // TODO: Add filter to validate if current user is the one modify the request status or not
    /// <summary>
    /// Update Request Status
    /// </summary> 
    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpPut("Request/UpdateStatus")]
    public async Task<ActionResult<LearningModuleRequest>> UpdateRequestStatus(UpdateRequestStatusDto dto)
    {
        return Ok(await _learningModuleService.UpdateRequestStatus(dto));
    }

    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpGet("Learners/GetAll")]
    public async Task<ActionResult<List<Learner>>> GetAllLearnersByLearningModuleId(int learningModuleId)
    {
        var user = await _contextService.GetAppUserAndThrow();
        return await _learningModuleService.GetAllLearnerInLearningModule(learningModuleId, user.Tutor.Id);

    }

    [Authorize(Roles = CustomRoles.GeneralUser)]
    [HttpGet("LearningModuleOfOneTutor/{tutorId}")]
    public async Task<ActionResult<List<LearningModule>>> GetAllLearningModuleOfOneTutor(Guid tutorId)
    {
        var learningModule = await _learningModuleService.GetAllLearningModuleOfOneTutor(tutorId);
        return Ok(learningModule);
    }

    [Authorize(Roles = CustomRoles.GeneralUser)]
    [HttpGet("AverageRatingOfTutor/{tutorId}")]
    public async Task<ActionResult<double>> GetAverageRatingOfTutorByAllLearningModule(Guid tutorId)
    {
        try
        {
            var averageRating = await _learningModuleService.GetAverageRatingOfTutorByAllLearningModule(tutorId);
            return Ok(averageRating);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [Authorize(Roles = CustomRoles.GeneralUser)]
    [HttpGet("NumberOfLearner")]
    public async Task<ActionResult<int>> GetNumberOfLearner(int learningModule)
    {
        return Ok(await _learningModuleService.GetNumberOfLearnersInAClass(learningModule));
    }
    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpPost("CreateQuestion")]
    public async Task<ActionResult> CreateQuestion(QuestionDto dto) {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.CreateQuestionForSesstion(dto, user));
    }
    [Authorize(Roles = CustomRoles.Learner)]
    [HttpPost("AnswerQuestion")]
    public async Task<ActionResult> AnswerQuestion(AnswerDto dto)
    {
        var user = await _contextService.GetAppUserAndThrow();
        return Ok(await _learningModuleService.AnswerQuestion(dto, user));
    }
    [Authorize(Roles = CustomRoles.Tutor)]
    [HttpPost("GradeAnswer")]
    public async Task<ActionResult> GradeAnswer(GradeAnswerDto dto) {
        return Ok(await _learningModuleService.Grade(dto));
    }
    [HttpPost("GetQuestionBySesstion")]
    public async Task<ActionResult<Question>> GetQuestionBySesstion(int id) {
        return Ok(await _learningModuleService.getQuestionBySession(id));
    }
    [HttpGet("GetAnswerByQuestion")]
    public async Task<ActionResult<List<Answer>>> GetAnswerByQuestion(int id) {
        return Ok(await _learningModuleService.GetAnswerByQuestion(id));
    }

}
