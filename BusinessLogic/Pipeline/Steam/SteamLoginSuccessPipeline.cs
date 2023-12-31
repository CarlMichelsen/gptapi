﻿using BusinessLogic.Pipeline.Shared;
using BusinessLogic.Pipeline.Steam.LoginSuccess;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline.Steam;

public class SteamLoginSuccessPipeline : Pipeline<ILoginPipelineParameters>
{
    public SteamLoginSuccessPipeline(
        ValidateSteamOAuthRecordStage validateOAuthRecordStage,
        AppendCookieHeaderStage appendCookieHeaderStage,
        DeriveSuccessRedirectUriStage deriveSuccessRedirectUriStage)
    {
        this.AddStage(validateOAuthRecordStage)
            .AddStage(appendCookieHeaderStage)
            .AddStage(deriveSuccessRedirectUriStage);
    }
}
