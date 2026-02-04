using Deepstaging.ToDo.Services;
using Deepstaging.ToDo.Services.Data;

namespace Deepstaging.ToDo;

[EffectsModule(typeof(IEmailService), Name = "Email")]
[EffectsModule(typeof(ISlackService), Name = "Slack")]
[EffectsModule(typeof(TodoDbContext), Name = "Database")]
public partial class RuntimeEffects;

[Runtime]
[Uses(typeof(RuntimeEffects))]
public partial class Runtime;