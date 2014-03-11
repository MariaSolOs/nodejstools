﻿/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Microsoft.NodejsTools.Debugger.Commands {
    sealed class ChangeLiveCommand : DebuggerCommand {
        private readonly Dictionary<string, object> _arguments;

        public ChangeLiveCommand(int id, NodeModule module) : base(id, "changelive") {
            // Wrap script contents as following https://github.com/joyent/node/blob/v0.10.26-release/src/node.js#L880
            string source = string.Format("{0}{1}{2}",
                NodeConstants.ScriptWrapBegin,
                module.Source,
                NodeConstants.ScriptWrapEnd);

            _arguments = new Dictionary<string, object> {
                { "script_id", module.Id },
                { "new_source", source },
                { "preview_only", false },
            };
        }

        protected override IDictionary<string, object> Arguments {
            get { return _arguments; }
        }

        public bool Updated { get; private set; }
        public bool StackModified { get; private set; }
        public bool NeedStepIn { get; private set; }

        public override void ProcessResponse(JObject response) {
            base.ProcessResponse(response);

            JToken result = response["body"]["result"];
            Updated = (bool)result["updated"];
            StackModified = (bool)result["stack_modified"];
            NeedStepIn = (bool)result["stack_update_needs_step_in"];
        }
    }
}