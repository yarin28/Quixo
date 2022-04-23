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

def save_all_files_to_one_file_with_file_names(file_list:List):
    big_file_string:String=""
    for file in file_list:
        with open (DIRECTORY+"/" +file,"r") as f:
            big_file_string +=file+"\n"+f.read()+"\n"
    print (big_file_string)
def main():
    print(os.getcwd())
    files = get_list_of_files(".cs")
    files = files +get_list_of_files(".xaml")
    save_all_files_to_one_file_with_file_names(files)
if __name__=="__main__":
    main()