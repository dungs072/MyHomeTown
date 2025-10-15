Hello everyone

- Coroutine should be separated in small functions (each function can only contain one loop) (More readable)

- Events should be written like: On + Object + V(ing/ed) or On + Adj (where object is it own). 
Ex: OnAgentSpawned
- properties that only used in class, no getter too will be added _ at the prefix of the var    name.
Ex: _count
- payload structure should be defined in the Connector folder
    - it must add 'able' at the suffix of the file name
    - if it is interface, it must add 'I' at the prefix of the file name
- Folder structures: 
    - Data folder: contains ScriptableObject components and types

- Task Designer
    - The step should be split only one time. After that, the children of that step will no longer be separated again.