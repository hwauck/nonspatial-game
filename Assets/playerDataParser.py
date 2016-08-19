import re		
file = open('PLAYER_DATA.csv', 'r')
file = open('STATUE.csv', 'w')
file = open('ICE.csv', 'w')
file = open('TILE.csv', 'w')
# this file will contain the playerData var for each session separated by newline characters
#first, separate by player
playerData = file.readlines();

parsePlayers()
writePlayerData()

#for every player, make an array of attempts
def parsePlayers():
	playerAttemptsList = [] #a list of lists of attempts
	numPlayers = len(playerData)
	for p in range(numPlayers):
		numAttepts = re.count('NEW_ATTEMPT,|NEW_GAME,', playerData[p])
		attempts = re.split('NEW_ATTEMPT,|NEW_GAME,', playerData[p])
		attempts[0] = numAttempts #replace empty string element with number of attempts
		playerAttemptsList.append(attempts)

		
def getStatueHeader():
	header = ''
	header.append('TOTAL_MOVES,TURNS,NUM_SQUARES_TRAVERSED,SQUARES_EXPLORED,NUM_REPEATED_SQUARES,')
	header.append('LEFT_SQUARES,RIGHT_SQUARES,TOP_SQUARES,BOTTOM_SQUARES,PLAYER_STATUE_COLLIDE,')
	header.append('PLAYER_BLOCKED_BY_STATUE,STATUES_BLOCK_EACH_OTHER,STATUES_COLLIDE,')
	header.append('STATUE_BLOCKED_BY_OFFSCREEN,STATUE_SQUARES_TRAVERSED,STATUE_SQUARES_EXPLORED,')
	header.append('STATUE_SQUARES_REPEATED,STATUE_LEFT_SQUARES,STATUE_RIGHT_SQUARES,')
	header.append('STATUE_TOP_SQUARES,STATUE_BOTTOM_SQUARES,ALL_MOVE,TWO_MOVE,ONE_MOVE,TOTAL_TIME')
	return header
	
	
#parse each attempt into a list to extract data
#add player's avg data across all attempts to csv string
def writePlayerData():
	statue_csv = ''
	ice_csv = ''
	tile_csv = ''
	for attemptList in playerAttemptsList:
		if(attemptList.startswith('statue')):
			statue_csv.append(getStatueHeader())
			statue_csv.append(getStatueDataAllAttempts(attemptList))
		elif (attemptList.startswith('ice')):
			ice_csv.append(getIceDataAllAttempts(attemptList))
		elif (attemptList.startswith('tile')):
			tile_csv.append(getTileDataAllAttempts(attemptList))
		else:
			print('Error: invalid game type')

	#write csv strings to files
	return

# returns string of all statue data attempts
def getStatueDataAllAttempts(attempts):
	NUM_COLS = 26
	statueData = ''
	numAttempts = attempts[0]
	totals = []
	for i in range(NUM_COLS):
		totals.append(0)
		
	for i in range(1:numAttempts):
		tempData = ''
		tempList = attempts[i].split(',')
		tempList.pop(0)
		tempList.pop(1)
		tempIndex = 3
		for j in range(NUM_COLS):
			totals[j] += tempList[tempIndex]
			tempIndex += 2

		#totals[6] = total_left_squares
		#totals[7] = total_right_squares
		#totals[8] = total_top_squares
		#totals[9] = total_bottom_squares
		#totals[18] = total_statue_left_squares
		#totals[19] = total_statue_right_squares
		#totals[20] = total_statue_top_squares
		#totals[21] = total_statue_bottom_squares
		if (totals[7] > 0): # [26]: left_right_symmetry
			totals.append(totals[6]/totals[7])
		else:
			totals.append(-1)
		if(totals[9] > 0): #[27]: top_bottom_symmetry
			totals.append(totals[8]/totals[9])
		else:
			totals.append(-1)
		if(totals[19] > 0): #[28]: statue_left_right_symmetry
			totals.append(totals[18]/totals[19])
		else:
			totals.append(-1)
		if(totals[21] > 0): #[29]: statue_top_bottom_symmetry
			totals.append(totals[20]/totals[21])
		else:
			totals.append(-1)
			
		#totals[0] = total_moves, totals[1] = total_turns, totals[2] = total_time_moving
		if(totals[0] > 0): #[30],[31]
			totals.append(totals[2]/totals[0]) #avg_time_per_move
			totals.append(totals[1]/totals[0]) #avg_turns_per_move
		else:
			totals.append(-1)
			totals.append(-1)
	
		for i in range(len(totals)):
			if(i != 2):
				statueData.append(totals[i] + ',')
				
		tempData = tempData[:-1] #get rid of last comma
		tempDate.append('\n')
		statueData.append(tempData)
	
	return statueData
	
def getStatueDataFirstAttempt(attempts):
	#TODO
	statueData = ''
	return statueData
	
def getIceDataAllAttempts(attempts):
	#TODO
	iceData = ''
	return iceData
	
def getIceDataFirstAttempt(attempts):
	#TODO
	iceData = ''
	return iceData
	
def getTileDataAllAttempts(attempts):
	#TODO
	tileData = ''
	return tileData

def getTileDataFirstAttempt(attempts):
	#TODO
	tileData = ''
	return tileData
	
# returns one row in the STATUE.csv data file	
def getStatueDataAvgAttempts(attempts):
	NUM_COLS = 26
	statueData = ''
	numAttempts = attempts[0]
	totals = []
	for i in range(NUM_COLS):
		totals.append(0)
		
	for i in range(1:numAttempts):
		tempList = attempts[i].split(',')
		tempList.pop(0)
		tempList.pop(1)
		tempIndex = 3
		for j in range(NUM_COLS):
			totals[j] += tempList[tempIndex]
			tempIndex += 2
	
	#can't be calculated in for loop
	#totals[6] = total_left_squares
	#totals[7] = total_right_squares
	#totals[8] = total_top_squares
	#totals[9] = total_bottom_squares
	#totals[18] = total_statue_left_squares
	#totals[19] = total_statue_right_squares
	#totals[20] = total_statue_top_squares
	#totals[21] = total_statue_bottom_squares
	if (totals[7] > 0): # [26]: left_right_symmetry
		totals.append(totals[6]/totals[7])
	else:
		totals.append(-1)
	if(totals[9] > 0): #[27]: top_bottom_symmetry
		totals.append(totals[8]/totals[9])
	else:
		totals.append(-1)
	if(totals[19] > 0): #[28]: statue_left_right_symmetry
		totals.append(totals[18]/totals[19])
	else:
		totals.append(-1)
	if(totals[21] > 0): #[29]: statue_top_bottom_symmetry
		totals.append(totals[20]/totals[21])
	else:
		totals.append(-1)
		
	#totals[0] = total_moves, totals[1] = total_turns, totals[2] = total_time_moving
	if(totals[0] > 0): #[30],[31]
		totals.append(totals[2]/totals[0]) #avg_time_per_move
		totals.append(totals[1]/totals[0]) #avg_turns_per_move
	else:
		totals.append(-1)
		totals.append(-1)
	
	#divide all except ratios by number of attempts
	# are we more interested in symmetry ratios per attempt or average over all games?
	for i in range(NUM_COLS):
		totals[i] /= numAttempts
	
	for i in range(len(totals)):
		if(i != 2):
			statueData.append(totals[i] + ',')
			
	statueData = statueData[:-1] #get rid of last comma
	return statueData