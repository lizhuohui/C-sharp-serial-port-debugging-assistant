
set CurrPath=%~dp0

cd %CurrPath%
rd /s /q .vs
attrib -h *.suo
DEL /F/S/Q *.suo

cd %CurrPath%First_Demo\
rd /s /q obj
rd /s /q bin


pause
