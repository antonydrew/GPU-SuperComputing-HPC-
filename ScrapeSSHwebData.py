import httplib
import urllib2
import json
import sys
import pickle
import math
from StringIO import StringIO
from xml.dom.minidom import parse, parseString
from json import loads

server = "www.southsidehealth.org"
conn = httplib.HTTPConnection(server, 80, timeout=30)
method = "GET"

body = "/mapData/cat_"
ext = ".json"
path = "C:/"


c = range(1,17)
cc = range(1,40)
count =0

for n in c:
     for x in cc:
            cs = str(n)
            ccs = str(x)
            try:
                if x < 10:
                    url = body+cs+"."+"0"+ccs+ext
                    url2 = cs+"0"+ccs+ext
                else:
                    url = body+cs+"."+ccs+ext
                    url2 = cs+ccs+ext
                print url
                
                
                conn.request(method, url)
                resp = conn.getresponse()
                answer = resp.read()
                jsonresp = loads(answer)
                for row in jsonresp:
                    count = count+1
                s = json.dumps(answer)
                f = open('\\tmp\o'+url2, 'a')
                f.write(answer)
                f.close()
                #print f
                #print jsonresp
                
            except:
                x = x+1

print count
conn.close()
