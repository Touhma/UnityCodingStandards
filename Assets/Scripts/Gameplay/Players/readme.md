Each Modules Are going to be split into multiples folder for clarity & organisation. 

It is important that the structure stay the same everywhere for ease of understanding. 


Components -> Will hold any IComponentData and derived

Factories -> Depending on how your project want to manage entities that could be a good alternative to aspect even if a bit more verbose 

Settings -> Will hold all the settings related stuff , definition of managed / unmanaged databases & Scripable object definitions. Think : all readonly stuff 

Systems -> All the logic of the game , the initialization, update of the state, etc ... Here it's only the manipulation of the data that is important.