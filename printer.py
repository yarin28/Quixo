#!/bin/python3
import os
from typing import List,AnyStr
DIRECTORY = "Quixo"
def get_list_of_files(file_type:AnyStr=".cs"):
    return_files = []
    files:list =  os.listdir(os.getcwd()+"/"+DIRECTORY)
    for file in files:
        if  file.endswith(file_type):
            return_files.append(file)
    return return_files

def get_list_of_files2(dirName):
    # create a list of file and sub directories 
    # names in the given directory 
    listOfFile = os.listdir(dirName)
    allFiles = list()
    # Iterate over all the entries
    for entry in listOfFile:
        # Create full path
        fullPath = os.path.join(dirName, entry)
        # If entry is a directory then get the list of files in this directory 
        if os.path.isdir(fullPath) and f"Evaluating" in fullPath:
            allFiles = allFiles + get_list_of_files2(fullPath)
        else:
            if ".cs" in fullPath or ".xaml" in fullPath:
                allFiles.append(fullPath)
                
    return allFiles

def save_all_files_to_one_file_with_file_names(file_list:List):
    big_file_string:String=""
    for file in file_list:
        with open (DIRECTORY+'\\' +file,"r") as f:
            big_file_string +=file+"\n"+f.read()
    print (big_file_string)

def print_files_in_list(file_list:List):
    for file in file_list:
        with open(file,"r") as f:
            print(f.read())

def save_file_content_to_big_string_including_file_names(file_list:List):
    big_file_string:String=""
    for file in file_list:
        with open (file,"r") as f:
            big_file_string +=file+"\n"+f.read()
    return big_file_string

def main():
    print(os.getcwd())
    files = get_list_of_files(".cs")
    files = files +get_list_of_files(".xaml")
    # save_all_files_to_one_file_with_file_names(files)
    files_with_subdirectory:List=get_list_of_files2(f"{os.getcwd()}\{DIRECTORY}")
    # print(files_with_subdirectory)
    with open("all_files_and_names.txt","w") as f:
        f.write(save_file_content_to_big_string_including_file_names(files_with_subdirectory))
if __name__=="__main__":
    main()