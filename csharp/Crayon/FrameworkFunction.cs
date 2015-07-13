﻿namespace Crayon
{
	internal enum FrameworkFunction
	{
		ABS = 1,
		ARCCOS,
		ARCSIN,
		ARCTAN,
		ARCTAN2,
		ASSERT,
		CHR,
		COS,
		CURRENT_TIME,
		FLOOR,

		GAME_CLOCK_TICK,
		GAME_INITIALIZE,
		GAME_INITIALIZE_SCREEN,
		GAME_INITIALIZE_SCREEN_SCALED,
		GAME_PUMP_EVENTS,
		GAME_SET_TITLE,

		GAMEPAD_BIND_ANALOG_AXIS,
		GAMEPAD_BIND_BUTTON,
		GAMEPAD_BIND_DIGITAL_AXIS,
		GAMEPAD_DISABLE_DEVICE,
		GAMEPAD_ENABLE_DEVICE,
		GAMEPAD_GET_DEVICES,
		GAMEPAD_GET_PUSHED_BUTTONS,

		GFX_BLIT_IMAGE,
		GFX_BLIT_IMAGE_PARTIAL,
		GFX_BLIT_IMAGE_PARTIAL_AT_SCALE,
		GFX_DRAW_ELLIPSE,
		GFX_DRAW_LINE,
		GFX_DRAW_RECTANGLE,
		GFX_FILL_SCREEN,
		GFX_FLIP_IMAGE,
		GFX_IMAGE_ERROR_CODE,
		GFX_IMAGE_GET,
		GFX_IMAGE_LOAD_FROM_RESOURCE,
		GFX_IMAGE_LOAD_FROM_WEB,
		GFX_IMAGE_LOAD_FROM_USER_DATA,
		GFX_IMAGE_POP_SCALE,
		GFX_IMAGE_PUSH_SCALE,
		GFX_IMAGE_SHEET_ERROR_CODE,
		GFX_IMAGE_SHEET_LOAD,
		GFX_IMAGE_SHEET_LOAD_PROGRESS,
		GFX_IMAGE_SHEET_LOADED,
		GFX_IS_IMAGE_LOADED,

		HTTP_REQUEST,

		IO_CURRENT_DIRECTORY,
		IO_DIRECTORY_LISTING,
		IO_FILE_READ_TEXT,
		IO_FILE_WRITE_TEXT,
		IO_IS_PATH_DIRECTORY,
		IO_PATH_EXISTS,
		IO_PATH_JOIN,

		ORD,
		PARSE_FLOAT,
		PARSE_INT,
		PARSE_JSON,
		PRINT,
		RANDOM,
		RESOURCE_READ_TEXT,

		SFX_GET_SOUND,
		SFX_IS_SOUND_LOADED,
		SFX_LOAD_SOUND_FROM_RESOURCE,
		SFX_PLAY_SOUND,

		SIN,
		TAN,
		TYPEOF,

		USER_DATA_DELETE_DIRECTORY,
		USER_DATA_DELETE_FILE,
		USER_DATA_DIRECTORY_LISTING,
		USER_DATA_FILE_READ_TEXT,
		USER_DATA_FILE_WRITE_TEXT,
		USER_DATA_MAKE_DIRECTORY,
		USER_DATA_IS_PATH_DIRECTORY,
		USER_DATA_PATH_EXISTS,
	}
}
