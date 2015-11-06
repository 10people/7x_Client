from time import gmtime, strftime
import os
import time
import string


f = open( "../Resources/_Data/Config/Config.txt" )
f_t = open( "../Resources/_Data/Config/Config~.txt", 'w' )

CONST_SPLITTER = ':'

for t_line in f:
    t_items = t_line.split( CONST_SPLITTER )
#   print "t_items: ", t_items

    t_len = len( t_items )
#   print "len: ", t_len

    if( t_len < 2 ):
	f_t.write( t_line )
        continue
    
    x = t_items[ 0 ]
    y = t_items[ 1 ]
#   print "x: ", x
#   print "y: ", y
    
    x = t_items[ 0 ].strip()
    y = t_items[ 1 ].strip()

#   print "Processed x: ", x
#   print "Processed y: ", y
    
    if x == "Version":
        f_t.write( x + CONST_SPLITTER + time.strftime('%m%d-%H%M',time.localtime()) + "\n" )        
    else:
        f_t.write( t_line )
        
f_t.close()
f.close()

os.remove( "../Resources/_Data/Config/Config.txt" )
os.rename( "../Resources/_Data/Config/Config~.txt", "../Resources/_Data/Config/Config.txt" )
