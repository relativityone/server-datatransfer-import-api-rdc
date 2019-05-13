@echo off

set filePath=%~1
set SIGNTOOL=%~2

FOR /L %%A IN (1,1,3) DO (
	echo SIGNTOOL: Checking if we need to sign file: "%filePath%"
	"%SIGNTOOL%" verify /pa /q "%filePath%"
	IF errorlevel 1 (
		echo SIGNTOOL: Attempting to sign file: "%filePath%" using http://timestamp.verisign.com/scripts/timstamp.dll
		"%SIGNTOOL%" sign /a /t http://timestamp.verisign.com/scripts/timstamp.dll /d Relativity /du http://www.kcura.com "%filePath%" 2> nul
		
		IF errorlevel 1 (
			echo SIGNTOOL: SIGNTOOL ERROR occured, retrying to sign file "%filePath%" using http://timestamp.comodoca.com/authenticode
			"%SIGNTOOL%" sign /a /t http://timestamp.comodoca.com/authenticode /d Relativity /du http://www.kcura.com "%filePath%" 2> nul
			
			IF errorlevel 1 (
				echo SIGNTOOL: SIGNTOOL ERROR occured, retrying to sign file: "%filePath%" using http://tsa.starfieldtech.com
				"%SIGNTOOL%" sign /a /t http://tsa.starfieldtech.com /d Relativity /du http://www.kcura.com "%filePath%" 2> nul
			) ELSE (
				GOTO SUCCEED
			)
		) ELSE (
			GOTO SUCCEED
		)
	) ELSE (
		echo SIGNTOOL: No need to sign an already signed file: "%filePath%"
		GOTO SUCCEED
	)
	sleep 1
)

echo exit_1
exit /b 1

:SUCCEED
echo succeed_exit_0
exit /b 0
