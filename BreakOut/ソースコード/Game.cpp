#include "DxLib.h"
#include <time.h>
#include <stdio.h>
#include <stdlib.h>

#define WIDTH 350
#define HEIGHT 480
#define pos_x 70
#define pos_y 0

#define ball_size 5
#define ball_speed 5
#define ball_add 5

#define bar_speed 5
#define bar_width 50
#define bar_height 15
#define boost_add 70

#define block_width 50
#define block_height 20
#define block_map_index_x 7
#define block_map_index_y 9
#define max_block 63

//関数
void Menu(void);
void Instructions(void);
void Init(void);
void InitBlocks(void);
void GameClear(void);
void GameOver(void);
void MoveBall(void);
void MoveBar(void);
void Collision(void);
void DrawBall(void);
void DrawBar(void);
void DrawField(void);
void DrawBlock(void);
void DrawBack(void);
void DrawScoreBox(void);
int GetKey(void);

//変数宣言
int state;
int count;

int bar_x;
int bar_y;

int ball_x;
int ball_y;
int rand_x;
int rand_y;

int speed_x;
int speed_y;
int boost_limit;
int boost_flag;

int score;
int last;

//カラー
int Color_White;
int Color_Black;
int Color_Red;
int Color_Green;
int Color_Blue;

//BGM・SE
int bgm;
int se_des;
int se_cancel;
int se_block;
int se_wall;
int se_boost;

//画像
int bar_image;
int ball_image;
int back_image1;
int back_image2;
int title_logo_image;
int UI_logo_image1;
int UI_logo_image2;
int GameOver_image;

int effect[6];

int key[256];

int selectnum = 0;
//メニュー画面用構造体
typedef struct {
	int x, y;
	char name[128];
} MenuElement_t;


//ブロック用構造体
struct BLOCKS {
	int x;
	int y;
	int flag;
	int color;
};
struct BLOCKS blocks[max_block];

//ブロック
int block_map[block_map_index_y][block_map_index_x] = {
	{1,1,1,1,1,1,1},
	{1,1,1,1,1,1,1},
	{0,0,0,0,0,0,0},
	{1,1,1,1,1,1,1},
	{1,1,1,1,1,1,1},
	{0,0,0,0,0,0,0},
	{1,1,1,1,1,1,1},
	{0,0,0,0,0,0,0},
	{0,0,0,0,0,0,0}
};


//main
int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int) {
	ChangeWindowMode(TRUE);
	DxLib_Init();
	SetDrawScreen(DX_SCREEN_BACK);

	bgm = LoadSoundMem("./BGM/nc175006.mp3");
	PlaySoundMem(bgm, DX_PLAYTYPE_LOOP);

	Init();
	InitBlocks();


	while (ScreenFlip() == 0
		&& ProcessMessage() == 0
		&& ClearDrawScreen() == 0
		&& GetKey() == 0) {

		count++;

		switch (state) {
		case 0:						//メニュー画面
			Menu();

			break;
		case 1:						//ゲーム画面

			DrawBack();

			MoveBar();
			MoveBall();
			Collision();
			DrawBall();
			DrawBar();
			DrawBlock();
			DrawField();
			DrawScoreBox();

			if (last <= 0) {
				score += 3000;
				state = 3;
			}

			break;
		case 2:						//ゲームオーバー
			DrawBack();

			DrawBar();
			DrawBlock();
			DrawField();
			DrawScoreBox();
			GameOver();

			break;
		case 3:						//ゲームクリア
			DrawBack();

			DrawBar();
			DrawBlock();
			DrawField();
			DrawScoreBox();
			GameClear();

			break;
		case 4:						//操作説明
			Instructions();

			break;
		default:
			break;
		}

	}

	DxLib_End();

	return 0;
}

//メニュー画面
void Menu() {
	DrawGraph(110, 100, title_logo_image, TRUE);

	MenuElement_t MenuElement[3] = {
				{280, 260, "Game Start"},
				{280, 290, "Instructions"},
				{280, 320, "End Game"},
	};

	if (key[KEY_INPUT_DOWN] == 1) {
		selectnum = (selectnum + 1) % 3;
	}

	//ゲームスタート
	if (key[KEY_INPUT_RETURN] == 1 && selectnum == 0) {
		PlaySoundMem(se_des, DX_PLAYTYPE_BACK);
		state = 1;
	}
	//操作説明
	if (key[KEY_INPUT_RETURN] == 1 && selectnum == 1) {
		PlaySoundMem(se_des, DX_PLAYTYPE_BACK);
		state = 4;
	}
	//ゲーム終了
	if (key[KEY_INPUT_RETURN] == 1 && selectnum == 2) {
		PlaySoundMem(se_des, DX_PLAYTYPE_BACK);

		DxLib_End();
	}

	//メニュー画面を描画
	for (int i = 0; i < 3; i++) {
		if (i == selectnum) {
			DrawFormatString(MenuElement[i].x - 20, MenuElement[i].y, GetColor(255, 255, 255), MenuElement[i].name);
		}
		else {
			DrawFormatString(MenuElement[i].x, MenuElement[i].y, GetColor(255, 255, 255), MenuElement[i].name);
		}
	}
}

//操作説明
void Instructions() {
	DrawBox(120, 80, 520, 400, Color_White, FALSE);

	DrawFormatString(260, 150, Color_White, "Instructions");

	DrawFormatString(160, 200, Color_White, "左右キー：移動");
	DrawFormatString(160, 230, Color_White, "Z：ブースト");

	DrawFormatString(130, 360, Color_White, " Press 'Enter' or 'Back' Key to move Menu");

	if (key[KEY_INPUT_RETURN] == 1) {
		PlaySoundMem(se_cancel, DX_PLAYTYPE_BACK);
		state = 0;
	}
	if (key[KEY_INPUT_BACK] == 1) {
		PlaySoundMem(se_cancel, DX_PLAYTYPE_BACK);
		state = 0;
	}
}

//開始処理
void Init() {
	state = 0;
	bar_x = 0;
	bar_y = HEIGHT - bar_height;
	ball_x = bar_width / 2;
	ball_y = (HEIGHT - bar_height) - ball_size;
	speed_x = ball_speed;
	speed_y = ball_speed;
	boost_limit = 1;
	boost_flag = 1;

	score = 0;
	last = 35;

	Color_White = GetColor(255, 255, 255);
	Color_Black = GetColor(0, 0, 0);
	Color_Red = GetColor(255, 0, 0);
	Color_Green = GetColor(0, 255, 0);
	Color_Blue = GetColor(0, 0, 255);

	se_des = LoadSoundMem("./SE/des.wav");
	se_cancel = LoadSoundMem("./SE/cancel.mp3");
	se_block = LoadSoundMem("./SE/block.mp3");
	se_wall = LoadSoundMem("./SE/wall.wav");
	se_boost = LoadSoundMem("./SE/boost.mp3");

	bar_image = LoadGraph("./Sprite/bar_image.png");
	ball_image = LoadGraph("./Sprite/ball_image.png");
	back_image1 = LoadGraph("./Sprite/back_image1.jpg");
	back_image2 = LoadGraph("./Sprite/back_image2.jpg");
	title_logo_image = LoadGraph("./Sprite/title_logo.png");
	UI_logo_image1 = LoadGraph("./Sprite/UI_logo1.png");
	UI_logo_image2 = LoadGraph("./Sprite/UI_logo2.png");
	GameOver_image = LoadGraph("./Sprite/GameOverCanvas.png");

	LoadDivGraph("./Sprite/gra_effect.bmp", 6, 3, 2, 240, 240, effect);
}

//ブロック開始処理
void InitBlocks() {
	int i, j;
	int count = 0;

	//ブロック描写
	for (i = 0; i < block_map_index_y; i++) {
		for (j = 0; j < block_map_index_x; j++) {
			if (block_map[i][j] != 0) {
				blocks[count].x = (block_width * j);
				blocks[count].y = (block_height * i);
				blocks[count].flag = 1;

				switch (block_map[i][j]) {		//色指定
				case 1:
					blocks[count].color = Color_Red;
					break;
				case 2:
					blocks[count].color = Color_Green;
					break;
				case 3:
					blocks[count].color = Color_Blue;
					break;
				default:
					break;
				}
			}
			count++;
		}
	}
}
//ゲームクリア
void GameClear() {
	DrawGraph(180, 195, GameOver_image, TRUE);
	DrawFormatString(pos_x + 220, 210, Color_White, "GAME CLEAR !!");
	DrawFormatString(pos_x + 140, 250, Color_White, " Press 'Back' Key to move Menu");

	if (key[KEY_INPUT_BACK] == 1) {
		PlaySoundMem(se_des, DX_PLAYTYPE_BACK);
		Init();
		InitBlocks();
		state = 0;
	}
}

//ゲームオーバー
void GameOver() {
	DrawGraph(180, 195, GameOver_image, TRUE);
	DrawFormatString(pos_x + 220, 210, Color_White, "GAME OVER");
	DrawFormatString(pos_x + 140, 240, Color_White, " Press 'Enter' Key to Retly");
	DrawFormatString(pos_x + 140, 260, Color_White, " Press 'Back' Key to move Menu");

	if (key[KEY_INPUT_RETURN] == 1) {
		PlaySoundMem(se_des, DX_PLAYTYPE_BACK);
		Init();
		InitBlocks();
		state = 1;
	}
	if (key[KEY_INPUT_BACK] == 1) {
		PlaySoundMem(se_cancel, DX_PLAYTYPE_BACK);
		Init();
		InitBlocks();
		state = 0;
	}
}

//ボールの移動処理
void MoveBall() {
	ball_x += speed_x;
	ball_y += speed_y;
}

//プレイヤーの移動処理
void MoveBar() {
	//ブースト処理
	if (key[KEY_INPUT_Z] && boost_flag && boost_limit > 0
		&& key[KEY_INPUT_RIGHT] && bar_x > boost_add) {
		PlaySoundMem(se_boost, DX_PLAYTYPE_BACK);
		boost_flag = 0;
		boost_limit--;

		bar_x += boost_add;
	}
	else if (key[KEY_INPUT_Z] && boost_flag && boost_limit > 0
		&& key[KEY_INPUT_LEFT] && bar_x > boost_add) {
		PlaySoundMem(se_boost, DX_PLAYTYPE_BACK);
		boost_flag = 0;
		boost_limit--;
		bar_x -= boost_add;
	}
	else if (!key[KEY_INPUT_Z]) {
		boost_flag = 1;
	}

	//通常移動処理
	if (key[KEY_INPUT_RIGHT] && (bar_x + bar_width) < WIDTH) {
		bar_x += bar_speed;
	}
	if (key[KEY_INPUT_LEFT] && bar_x > 0) {
		bar_x -= bar_speed;
	}
}

//当たり判定
void Collision() {
	int count = 0;

	//画面右端の当たり判定
	if (ball_x >= WIDTH) {
		PlaySoundMem(se_wall, DX_PLAYTYPE_BACK);
		ball_x = WIDTH - ball_size;
		speed_x *= -1;
	}
	//画面左端の当たり判定
	if (ball_x <= 0) {
		PlaySoundMem(se_wall, DX_PLAYTYPE_BACK);
		ball_x = ball_size;
		speed_x *= -1;
	}
	//画面上部の当たり判定
	if (ball_y <= 0) {
		PlaySoundMem(se_wall, DX_PLAYTYPE_BACK);
		ball_y = ball_size;
		speed_y *= -1;
	}

	//ゲームオーバー判定
	if (ball_y >= HEIGHT) {
		state = 2;
	}

	//プレイヤーと接触判定
	if (ball_x > bar_x && ball_x < (bar_x + bar_width) && ball_y >= bar_y) {
		PlaySoundMem(se_wall, DX_PLAYTYPE_BACK);
		ball_y = bar_y - ball_size;
		speed_y *= -1;
	}

	//当たり判定
	for (count = 0;count < max_block;count++) {
		if (blocks[count].flag == 1) {
			if (ball_x >= blocks[count].x && ball_x <= blocks[count].x + block_width) {
				//上面当たり判定
				if (ball_y >= blocks[count].y && ball_y <= blocks[count].y + 3) {
					PlaySoundMem(se_block, DX_PLAYTYPE_BACK);
					ball_y = blocks[count].y;
					speed_y *= -1;
					blocks[count].flag = 0;
					score += 100;
					last--;

					break;
				}
				//下面当たり判定
				else if (ball_y <= blocks[count].y + block_height && ball_y >= blocks[count].y + block_height - 3) {
					PlaySoundMem(se_block, DX_PLAYTYPE_BACK);
					ball_y = blocks[count].y + block_height;
					speed_y *= -1;
					blocks[count].flag = 0;
					score += 100;
					last--;

					break;
				}
			}
			else if (ball_y >= blocks[count].y && ball_y <= blocks[count].y + block_height) {
				//左面当たり判定
				if (ball_x >= blocks[count].x && ball_x <= blocks[count].x + 3) {
					PlaySoundMem(se_block, DX_PLAYTYPE_BACK);
					ball_x = blocks[count].x;
					speed_x *= -1;
					blocks[count].flag = 0;
					score += 100;
					last--;

					break;
				}
				//右面当たり判定
				else if (ball_x <= blocks[count].x + block_width && ball_x >= blocks[count].x + block_width - 3) {
					ball_x = blocks[count].x + block_width;
					PlaySoundMem(se_block, DX_PLAYTYPE_BACK);
					speed_x *= -1;
					blocks[count].flag = 0;
					score += 100;
					last--;

					break;
				}
			}
		}
	}
}

//ボールの描写
void DrawBall() {
	DrawCircle(pos_x + ball_x, ball_y, ball_size, Color_White, TRUE);
}

//プレイヤーの描写
void DrawBar() {
	DrawGraph(pos_x + bar_x, bar_y, bar_image, TRUE);
}

//
void DrawField() {
	DrawBox(pos_x + 0, 0, pos_x + WIDTH, HEIGHT, Color_White, FALSE);
}

//ブロックの描写
void DrawBlock() {
	int count = 0;

	for (count = 0;count < max_block;count++) {
		if (blocks[count].flag == 1) {
			DrawBox(pos_x + blocks[count].x, blocks[count].y, pos_x + blocks[count].x + block_width,
				blocks[count].y + block_height, blocks[count].color, TRUE);
			DrawBox(pos_x + blocks[count].x, blocks[count].y, pos_x + blocks[count].x + block_width,
				blocks[count].y + block_height, Color_Black, FALSE);
		}
	}
}

//背景表示
void DrawBack() {
	DrawGraph(0, 0, back_image2, TRUE);
	DrawGraph(pos_x, 0, back_image1, FALSE);
}


//スコア表示
void DrawScoreBox() {
	DrawGraph(450, 20, UI_logo_image1, TRUE);
	DrawGraph(480, 70, UI_logo_image2, TRUE);

	DrawFormatString(465, 150, Color_Red, "SCORE");
	DrawFormatString(505, 170, Color_White, "%d", score);
	DrawFormatString(465, 200, Color_Red, "REMAINING");
	DrawFormatString(495, 220, Color_White, "%d", last);
	DrawFormatString(465, 250, Color_Blue, "BOOST");
	DrawFormatString(535, 250, Color_White, "%d", boost_limit);

	DrawFormatString(495, 380, Color_Red, "ROUND");
	DrawFormatString(515, 400, Color_White, "1");
}

//キー
int GetKey() {
	char keys[256];
	GetHitKeyStateAll(keys);

	for (int i = 0; i < 256; i++) {
		if (keys[i] != 0) {
			key[i]++;
		}
		else {
			key[i] = 0;
		}
	}
	return 0;
}