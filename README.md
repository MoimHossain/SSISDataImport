# Introduction SSIS Data Import

SQL server 2005 Integration Service ships with a rich set of APIs and in this article I am going to show you a way in which you can create and execute an SSIS package programmatically from C# code.

The codebase originally was written and [published in a Code Project article](http://www.codeproject.com/Articles/17497/Importing-data-with-the-SSIS-Object-model).

![Image](http://www.codeproject.com/KB/database/SSISProgramming/screenShot.JPG)


# Using the code

In the code base there are two projects. The class library (ImportLib) that implements the SSIS package creation tasks and the Windows form application (Import) which is actually a DEMO/consumer application that uses the library as reference and initiates an Import Job. So exploring the library should be the one to inspect.

# Points of Interest

No code has been written that can perform any transformations during importing.
If you need to perform some work/transform/modify on the data before it is imported into the destination data store, you need to do it for yourself. However,
there is an [article](http://www.codeproject.com/Articles/18853/Digging-SSIS-object-model) where I have explained how to do that.  


# License 

The associated source code and files, is licensed under MIT. 


# Feedback

You're welcome to use/modify/enhance/PullRequest the source code as it fits. 