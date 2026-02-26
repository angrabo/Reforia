## Adding a new module (step by step)

### Goal
You want to add a new package of RPC functions + services, without touching the hosting setup.

### Steps
1. **Create a new project/module** (e.g. `MyFeatureModule`).
2. Add an extension class in this style:
    - method `AddMyFeatureModule(this RpcBuilder builder)`:
        - registers services into `builder.Services`,
        - registers functions from the module assembly (scanning).

3. In `Program.cs`, append the module to the chain:
    - `builder.Services.AddRpc().AddMyFeatureModule()...`

### Checklist (what the module should do)
- [ ] Register dependencies in DI (Scoped/Singleton depending on the need)
- [ ] Register RPC functions (assembly scanning / manual registration)
- [ ] (optional) keep its own DTO contracts in one place
- [ ] (optional) module tests

### Tips
- Ensure function names are unique (so the dispatcher doesn’t get collisions).