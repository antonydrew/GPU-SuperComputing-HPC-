##import urllib2
##from xml.dom.minidom import parseString
##file = urllib2.urlopen('https://data.cityofchicago.org/api/file_data/VfBbNlHqUeka_pU8Ms5b12zm72-T48hGRcFHH-oEgwM?filename=Kmlcommunityareas.kmlhttp://www.somedomain.com/somexmlfile.xml')
##data = file.read()
##file.close()
##dom = parseString(data)
##xmlTag = dom.getElementsByTagName('tagName')
##xmlData=xmlTag.replace('<tagName>','').replace('</tagName>','')
##print xmlTag
##print xmlData


##f = open('/home/adrew/Downloads/Kmlcommunityareas.kml', 'r')
##print f
##l = f.readlines()
##print l
##for line in l:
##    print line
###print f.readlines()

from xml.dom.minidom import parse, parseString

dom1 = parse('/home/adrew/Downloads/Kmlcommunityareas.kml') # parse an XML file by name
print dom1
