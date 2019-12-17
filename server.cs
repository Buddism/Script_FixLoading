$firstWordCheck["+-EVENT"] = 1;
$firstWordCheck["+-NTOBJECTNAME"] = 1;
$firstWordCheck["+-EMITTER"] = 1;
$firstWordCheck["+-ITEM"] = 1;
$firstWordCheck["+-AUDIOEMITTER"] = 1;
$firstWordCheck["+-VEHICLE"] = 1;
$firstWordCheck["+-OWNER"] = 1;
$firstWordCheck["Linecount"] = 1;

package fixLoading
{
	function ServerLoadSaveFile_End()
	{
		$ServerLoadFileObjALT.delete();
		return parent::ServerLoadSaveFile_End();
	}
	function ServerLoadSaveFile_Start(%filename)
	{
		if ($Game::MissionCleaningUp)
		{
			parent::ServerLoadSaveFile_Start(%filename);
			return;
		}

		$ServerLoadFileObjALT = new FileObject();
		if (isFile(%filename))
			$ServerLoadFileObjALT.openForRead(%filename);
		else
			$ServerLoadFileObjALT.openForRead("base/server/temp/temp.bls");

		$ServerLoadFileObjALT.readLine();
		%lineCount = $ServerLoadFileObjALT.readLine();
		for(%i = 0; %i < %lineCount+64; %i++)
			$ServerLoadFileObjALT.readLine();

		return parent::ServerLoadSaveFile_Start(%filename);
	}

	function ServerLoadSaveFile_Tick()
	{
		if(isObject(ServerConnection))
			if(!ServerConnection.isLocal())
				return parent::ServerLoadSaveFile_Tick();

		%line = $ServerLoadFileObjALT.readLine();
		if(trim(%line) $= "")
			return parent::ServerLoadSaveFile_Tick();

		%firstWord = getWord(%line, 0);
		if(!$firstWordCheck[%firstWord] && strstr(%line, "\"") <= 0)
		{
			echo("BAD UI: "SPC %line);
			$Server_LoadFileObj.readLine();
			$LoadSaveFile_Tick_Schedule = schedule(0, 0, ServerLoadSaveFile_Tick);
			return;
		}

		return parent::ServerLoadSaveFile_Tick();
	}
};
activatePackage(fixLoading);

function serverCmdFixLoading(%client)
{
	if(%client.isAdmin)
	{
		deactivatePackage(fixLoading);
		activatePackage(fixLoading);
		resetAllOpCallFunc();
		
		announce(%client.getPlayerName() SPC " fixed the loading package");
	}
}
