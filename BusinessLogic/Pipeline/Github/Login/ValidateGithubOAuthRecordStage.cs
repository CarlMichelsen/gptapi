﻿using Domain.Entity;
using Domain.Exception;
using Domain.Pipeline;
using Domain.Service;
using Interface.Pipeline;
using Interface.Service;

namespace BusinessLogic;

public class ValidateGithubOAuthRecordStage : IPipelineStage<ILoginPipelineParameters>
{
    private readonly IOAuthRecordValidatorService oAuthRecordValidatorService;

    public ValidateGithubOAuthRecordStage(
        IOAuthRecordValidatorService oAuthRecordValidatorService)
    {
        this.oAuthRecordValidatorService = oAuthRecordValidatorService;
    }

    public async Task<ILoginPipelineParameters> Process(ILoginPipelineParameters input, CancellationToken cancellationToken)
    {
        var oAuthRecordValidationResult = await this.oAuthRecordValidatorService.ValidateOAuthRecord(
            input.OAuthRecordId,
            input.AccessToken,
            AuthMethods.Github);
        
        return oAuthRecordValidationResult.Match(
            (oAuthRecordValidatorResult) => this.HandlePipelineParameters(input, oAuthRecordValidatorResult),
            (error) => throw new OAuthException(error));
    }

    private ILoginPipelineParameters HandlePipelineParameters(
        ILoginPipelineParameters input,
        OAuthRecordValidatorResult oAuthRecordValidatorResult)
    {
        input.UserId = oAuthRecordValidatorResult.OAuthRecord.UserId;
        input.UserProfileId = oAuthRecordValidatorResult.UserProfileId;
        return input;
    }
}
