CSC = mcs
MONO = mono

test2: test2RngStream.cs RngStream.cs test2RngStream.res
	$(CSC) test2RngStream.cs RngStream.cs
	$(MONO) test2RngStream.exe | tee test.res
	@echo
	@echo Compare with the reference test output:
	diff -u test2RngStream.res test.res
	@rm test.res # tidy up

test: testRngStream.cs RngStream.cs
	$(CSC) testRngStream.cs RngStream.cs
	$(MONO) testRngStream.exe

test3: test3RngStream.cs RngStream.cs
	$(CSC) test3RngStream.cs RngStream.cs
	$(MONO) test3RngStream.exe
	R -q -f test3RngStream.R

test2dotnet:
	dotnet clean
	dotnet build /p:StartupObject=test2RngStream
	dotnet run

testdotnet:
	dotnet clean
	dotnet build /p:StartupObject=testRngStream
	dotnet run

clean:
	rm -f test.res
	rm -f testRngStream.exe
	rm -f test2RngStream.exe
	rm -f test3RngStream.exe

