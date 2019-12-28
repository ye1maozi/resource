# -*- coding: utf-8 -*- 
import xdrlib, sys
import xlrd
import os
import os.path

def isinttype(val):
	return type(val) == type(1.0) and val == int(val)

def convert(sourceFile):
	excl = xlrd.open_workbook(sourceFile)
	sheet = excl.sheets()[0]
	titleValues = sheet.row_values(0)
	
	outFile = "./csvfiles/" + sourceFile[:-5] + ".csv"
	csvfile = open(outFile, "w")
	csvline = ','.join(titleValues)
	csvfile.write(csvline + "\n")
	for m in xrange(2, sheet.nrows):
		csvline = ''
		for n in xrange(0, sheet.ncols):
			fieldvalue = sheet.row_values(m)[n]
			if isinttype(fieldvalue):
				csvline += "%s," % int(fieldvalue)
			else:
				csvline += "%s," % str(fieldvalue).replace(',', '，').replace('\n', '\\n').replace('\r', '\\r')
		csvline = csvline[:-1]
		if csvline != (',' * (sheet.ncols - 1)):
			csvfile.write(csvline + "\n")
	csvfile.close()

def main():
	os.system('rd /s/q csvfiles')
	os.system('mkdir csvfiles')
	for file in os.listdir('./'):
		if file[0] != '~' and (file[-5:] == '.xlsx' or file[-5:] == '.xlsx'):
			convert(file)

reload(sys)
sys.setdefaultencoding("utf-8")			
if __name__ == "__main__":
	main()