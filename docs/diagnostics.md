# Diagnostics Reference

All diagnostic IDs reported by Deepstaging analyzers.

## Effects Module (DSEFX / DSRT)

### EffectsModule Diagnostics

| ID | Severity | Description | Fix |
|----|----------|-------------|-----|
| DSEFX01 | Error | EffectsModule class must be partial | Add `partial` modifier |
| DSEFX02 | Warning | EffectsModule class should be sealed | Add `sealed` modifier |
| DSEFX03 | Warning | EffectsModule target should be an interface | — |
| DSEFX04 | Error | Duplicate EffectsModule target type | — |
| DSEFX05 | Error | Excluded method not found on target | — |
| DSEFX06 | Error | IncludeOnly method not found on target | — |
| DSEFX07 | Warning | EffectsModule target has no methods | — |

### Runtime Diagnostics

| ID | Severity | Description | Fix |
|----|----------|-------------|-----|
| DSRT01 | Error | Runtime class must be partial | Add `partial` modifier |
| DSRT02 | Error | Uses attribute requires Runtime attribute | — |
| DSRT03 | Error | Uses target must be an EffectsModule | — |
| DSRT04 | Info | Effects module available but not referenced by runtime | — |

## Typed IDs (DSID)

| ID | Severity | Description | Fix |
|----|----------|-------------|-----|
| DSID01 | Error | TypedId struct must be partial | Add `partial` modifier |
| DSID02 | Warning | TypedId struct should be readonly | Add `readonly` modifier |

## Configuration (DSCFG)

| ID | Severity | Description | Fix |
|----|----------|-------------|-----|
| DSCFG01 | Error | ConfigProvider class must be partial | Add `partial` modifier |
| DSCFG02 | Warning | ConfigProvider class should be sealed | Add `sealed` modifier |
| DSCFG03 | Error | Section name could not be inferred | — |
| DSCFG04 | Warning | Exposed type has no public instance properties | — |
| DSCFG05 | Warning | Property appears to contain secrets | Add `[Secret]` attribute |
| DSCFG06 | Info/Warning | Configuration files missing or out of date | Generate configuration files |
| DSCFG07 | Error | `[Secret]` properties exist but no `UserSecretsId` | Add `UserSecretsId` |

## HTTP Client (DSHTTP)

| ID | Severity | Description | Fix |
|----|----------|-------------|-----|
| DSHTTP01 | Error | HttpClient class must be partial | Add `partial` modifier |
| DSHTTP02 | Error | HTTP method must be partial | — |
| DSHTTP03 | Error | HTTP method must not return Task | — |
| DSHTTP04 | Error | HTTP path must not be empty | — |
