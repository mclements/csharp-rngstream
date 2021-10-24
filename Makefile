CSC = mcs
MONO = mono

test2: test2RngStream.cs rngstream.cs test2RngStream.res
	$(CSC) test2RngStream.cs rngstream.cs
	$(MONO) test2RngStream.exe | tee test.res
	@echo
	@echo Compare with the reference test output:
	diff -u test2RngStream.res test.res
	@rm test.res # tidy up

test: testRngStream.cs rngstream.cs
	$(CSC) testRngStream.cs rngstream.cs
	$(MONO) testRngStream.exe

test3: test3RngStream.cs rngstream.cs
	$(CSC) test3RngStream.cs rngstream.cs
	$(MONO) test3RngStream.exe

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

