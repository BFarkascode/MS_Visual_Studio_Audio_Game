using System;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using WMPLib;


//v0.1: First version of application
//v0.2: Added Advanced mode
//v0.3: Reworked snippet position placing for advacned mode
        //added response playback before success


//what to do?
//add validation for advanced mode
//add button click selector for word position
    //currentl advanced mode activates all elements and distributes snippets randomly to buttons
    //no playback, no validation. no position read-in
        //dropdown or clicking counter?

namespace NumeriqueApp_v3
{
    public partial class Form1 : Form
    {
        string[] paths;                                             //this is the file path array for each audio

        int first_word_btn1_val;                                    //these will be the button values we then use to get the paths when playing back the sounds        
        int first_word_btn2_val;
        int first_word_select;
        int second_word_btn1_val;
        int second_word_btn2_val;
        int second_word_select;
        int third_word_btn1_val;
        int third_word_btn2_val;
        int third_word_select;
        int fourth_word_btn1_val;
        int fourth_word_btn2_val;
        int fourth_word_select;
        int fifth_word_btn1_val;
        int fifth_word_btn2_val;
        int fifth_word_select;
        int sixth_word_btn1_val;
        int sixth_word_btn2_val;
        int sixth_word_select;
        int seventh_word_btn1_val;
        int seventh_word_btn2_val;
        int seventh_word_select;
        int eigtht_word_btn1_val;
        int eigtht_word_btn2_val;
        int eigtht_word_select;

        bool first_select_correct = false;                                                    //selection feedback
        bool second_select_correct = false;
        bool third_select_correct = false;
        bool fourth_select_correct = false;
        bool fifth_select_correct = false;
        bool sixth_select_correct = false;
        bool seventh_select_correct = false;
        bool eigtht_select_correct = false;

        bool first_word_correct = false;                                                    //selection feedback
        bool second_word_correct = false;
        bool third_word_correct = false;
        bool fourth_word_correct = false;
        bool fifth_word_correct = false;
        bool sixth_word_correct = false;
        bool seventh_word_correct = false;
        bool eigtht_word_correct = false;

        int audio_number;                                                                  //which audio we select from the pot
        int word_number;                                                                    //how many word we have in the audio

        int number_of_tests = 10;                                                            //this is what sets the number of questions
                                                                                            //maximum test number is 10!(see below)
                                                                                            //adjust according to test difficulty

        int number_of_audio_files = 0;                                                      //this is the number of audios available for the app to choose from
                                                                                            //this is read out by checking the solutions.txt file and counting the lines

        int[,] audio_array = { { 1 , 0 }, { 2 , 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 } };       //this is where we store the existing combinations
        int step_number = 0;                                                                                                                    //this is where we store our progress
        int correct_answer_cnt = 0;

        IWMPPlaylist WrongAnswerPlaylist;

        bool wrong_answer_published = false;

        string exe_location;                                                                 //this is the path where the app is stored

        string[] solutions;                                                                  //this will be the solutions in text

        int[] snippet_pos_array;

        private class ComboItem                                                             //for dropdown selection
        {
            public int ID { get; set; }
            public string Text { get; set; }
        };

        private class snippet_info                                                             //for dropdown selection
        {
            public string word_pos { get; set; }
            public string path { get; set; }

        };

        snippet_info[] snippets = new snippet_info[8];

        //pos select value
        int[] pos_sel = new int[8];
        int pos_sel1;
        int pos_sel2;
        int pos_sel3;
        int pos_sel4;
        int pos_sel5;
        int pos_sel6;
        int pos_sel7;
        int pos_sel8;

        //aid counter
        int aid_cnt;

        public Form1()
        {
            InitializeComponent();

 //           snippet_info[] snippets = new snippet_info[8];

            //we define the snippet information
            snippets[0] = new snippet_info { word_pos = "0", path = "Lorem ipsum" };
            snippets[1] = new snippet_info { word_pos = "0", path = "Lorem ipsum" };
            snippets[2] = new snippet_info { word_pos = "0", path = "Lorem ipsum" };
            snippets[3] = new snippet_info { word_pos = "0", path = "Lorem ipsum" };
            snippets[4] = new snippet_info { word_pos = "0", path = "Lorem ipsum" };
            snippets[5] = new snippet_info { word_pos = "0", path = "Lorem ipsum" };
            snippets[6] = new snippet_info { word_pos = "0", path = "Lorem ipsum" };
            snippets[7] = new snippet_info { word_pos = "0", path = "Lorem ipsum" };

            //we define the snippet position counters
            pos_sel1 = 1;
            pos_sel2 = 1;
            pos_sel3 = 1;
            pos_sel4 = 1;
            pos_sel5 = 1;
            pos_sel6 = 1;
            pos_sel7 = 1;
            pos_sel8 = 1;

            for (int i = 0; i < 8; i++) {

                pos_sel[i] = 1;

            }

            //aid counter for advanced mode
            aid_cnt = 0;

            //difficulty selector
            cBox_difficulty.DataSource = new ComboItem[] {                        //we define the dropdown menu
                                                                                    //this is to be removed from production and external version

                   new ComboItem{ ID = 1, Text = "Beginner"},
                   new ComboItem{ ID = 2, Text = "Advanced"}

            };

            cBox_difficulty.DisplayMember = "Text";
            cBox_difficulty.Enabled = true;

            //note: the playlist will be left on the user's PC wherever the playlists are stored
            //reason: the library folder can not be set using C#
            IWMPPlaylistArray media_array = media_player.playlistCollection.getByName("wrong_answer_list");          //we read in ALL playlists with simialr names
                                                            //Note: this function finds EVERY playlist that shares the same name, even the ones that are already removed

            if (media_array.count == 0)                                                                         //if there is none
            {

                WrongAnswerPlaylist = media_player.playlistCollection.newPlaylist("wrong_answer_list");              //we create it

            }
            else {

                WrongAnswerPlaylist = media_array.Item(0);                                                      //if yes, we read it in
                WrongAnswerPlaylist.clear();                                                                    //we wipe the playlist

            }

            //we select the playlist
            media_player.currentPlaylist = WrongAnswerPlaylist;

            //we update the score
            txt_score.Text = "0/" + number_of_tests.ToString();

            //enable the START button
            btn_open.Enabled = true;

            //disable the solutuion button
            btn_solution.Enabled = false;

            //disable all the word selector buttons
            first_word_btn1.Enabled = false;
            first_word_btn2.Enabled = false;
            second_word_btn1.Enabled = false;
            second_word_btn2.Enabled = false;
            third_word_btn1.Enabled = false;
            third_word_btn2.Enabled = false;
            fourth_word_btn1.Enabled = false;
            fourth_word_btn2.Enabled = false;
            fifth_word_btn1.Enabled = false;
            fifth_word_btn2.Enabled = false;
            sixth_word_btn1.Enabled = false;
            sixth_word_btn2.Enabled = false;
            seventh_word_btn1.Enabled = false;
            seventh_word_btn2.Enabled = false;
            eighth_word_btn1.Enabled = false;
            eighth_word_btn2.Enabled = false;


            //uniform colour
            first_word_btn1.BackColor = Color.LightGray;
            first_word_btn2.BackColor = Color.LightGray;
            second_word_btn1.BackColor = Color.LightGray;
            second_word_btn2.BackColor = Color.LightGray;
            third_word_btn1.BackColor = Color.LightGray;
            third_word_btn2.BackColor = Color.LightGray;
            fourth_word_btn1.BackColor = Color.LightGray;
            fourth_word_btn2.BackColor = Color.LightGray;
            fifth_word_btn1.BackColor = Color.LightGray;
            fifth_word_btn2.BackColor = Color.LightGray;
            sixth_word_btn1.BackColor = Color.LightGray;
            sixth_word_btn2.BackColor = Color.LightGray;
            seventh_word_btn1.BackColor = Color.LightGray;
            seventh_word_btn2.BackColor = Color.LightGray;
            eighth_word_btn1.BackColor = Color.LightGray;
            eighth_word_btn2.BackColor = Color.LightGray;

            //disable the solution and the score
            txt_solution.Enabled = false;
            txt_score.Enabled = false;

            //set the volume
            track_volume.Value = 50;

            //read in the solutions
            exe_location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

 /*           string solutions_file_path = exe_location + @"\Audio_files\Beginner\Solutions.txt";

            // To read a text file line by line
            if (File.Exists(solutions_file_path))
            {
                // Store each line in array of strings
                solutions = File.ReadAllLines(solutions_file_path);

                foreach (string ln in solutions) {
                    number_of_audio_files++;                        //we check how many audios we have by checking how many solutions are stored in the solutions file
                }
            }
            else {

                txt_solution.Text = "Error! No solutions.txt file found.";

            }*/

            //snippet array
            //currently set to maixmum 8 snippet pairs
            snippet_pos_array = new int[8];

            //disable snippet position selectors
            btn_pos_sel1.Enabled = false;
            btn_pos_sel2.Enabled = false;
            btn_pos_sel3.Enabled = false;
            btn_pos_sel4.Enabled = false;
            btn_pos_sel5.Enabled = false;
            btn_pos_sel6.Enabled = false;
            btn_pos_sel7.Enabled = false;
            btn_pos_sel8.Enabled = false;

            //disable pass button
            btn_pass.Enabled = false;

        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
 
        private void track_volume_Scroll(object sender, EventArgs e)
        {
            media_player.settings.volume = track_volume.Value;

        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            wrong_answer_published = false;

            if ((btn_open.Text == "Next") | (btn_open.Text == "Start"))
            {

                cBox_difficulty.Enabled = false;
                txt_solution.Enabled = true;
                txt_score.Enabled = true;

                btn_open.Text = "Next";
                txt_solution.Clear();

                //implement here a check so we won't read in the same audio twice during the same game

                Random rnd_audio = new Random();                                 //we randomly select the audio to read in

                bool audio_loaded_already = true;                                 //here we check if we have already loaded this audio before or not

                bool no_more_audios = false;

                int audio_check_cnt = 0;

                number_of_audio_files = 0;

                if (cBox_difficulty.Text == "Beginner")
                {

                    string beg_solutions_file_path = exe_location + @"\Audio_files\Beginner\beg_solutions.txt";

                    // To read a text file line by line
                    if (File.Exists(beg_solutions_file_path))
                    {
                        // Store each line in array of strings
                        solutions = File.ReadAllLines(beg_solutions_file_path);

                        foreach (string ln in solutions)
                        {
                            number_of_audio_files++;                        //we check how many audios we have by checking how many solutions are stored in the solutions file
                        }
                    }
                    else
                    {

                        txt_solution.Text = "Error! No solutions text file found.";
                        btn_open.Enabled = false;
                        while (true) ;

                    }

                }
                else {

                    string adv_solutions_file_path = exe_location + @"\Audio_files\Advanced\adv_solutions.txt";

                    // To read a text file line by line
                    if (File.Exists(adv_solutions_file_path))
                    {
                        // Store each line in array of strings
                        solutions = File.ReadAllLines(adv_solutions_file_path);

                        foreach (string ln in solutions)
                        {
                            number_of_audio_files++;                        //we check how many audios we have by checking how many solutions are stored in the solutions file
                        }
                    }
                    else
                    {

                        txt_solution.Text = "Error! No solutions text file found.";
                        btn_open.Enabled = false;
                        while (true) ;

                    }


                }

                while ((audio_loaded_already == true) & (no_more_audios == false))
                    {

                        if (audio_check_cnt < 20)                                      //we search for an appropraite audio for 20 times
                        {
                            no_more_audios = false;

                        }
                        else
                        {

                            no_more_audios = true;                               //if we have searched 20 times, we break the loop

                        }

                        audio_loaded_already = false;

                        audio_number = rnd_audio.Next(1, (number_of_audio_files + 1));        //we search amongst all avaialbe audios

                        for (int i = 0; i < number_of_tests; i++)
                        {


                            if (audio_array[i, 1] == audio_number)
                            {

                                audio_loaded_already = true;

                            }
                            else
                            {

                                //do nothing

                            }

                        }

                        audio_check_cnt++;

                    }


                if (step_number == number_of_tests)
                    {
                        txt_solution.Text = "Finished";
                        btn_open.Text = " ";
                        btn_open.Enabled = false;

                        btn_pos_sel1.Text = "";
                        btn_pos_sel2.Text = "";
                        btn_pos_sel3.Text = "";
                        btn_pos_sel4.Text = "";
                        btn_pos_sel5.Text = "";
                        btn_pos_sel6.Text = "";
                        btn_pos_sel7.Text = "";
                        btn_pos_sel8.Text = "";

                        btn_pos_sel1.Enabled = false;
                        btn_pos_sel2.Enabled = false;
                        btn_pos_sel3.Enabled = false;
                        btn_pos_sel4.Enabled = false;
                        btn_pos_sel5.Enabled = false;
                        btn_pos_sel6.Enabled = false;
                        btn_pos_sel7.Enabled = false;
                        btn_pos_sel8.Enabled = false;

                        txt_solution.Enabled = false;
                        txt_score.Enabled = false;


                    }
                 else
                    {

                        if (no_more_audios == true)                                      //normally we should never get here since we will have more audios than searches
                        {

                            txt_solution.Text = "No more audio left...";
                            btn_open.Text = " ";
                            btn_open.Enabled = false;

                            btn_pos_sel1.Text = "";
                            btn_pos_sel2.Text = "";
                            btn_pos_sel3.Text = "";
                            btn_pos_sel4.Text = "";
                            btn_pos_sel5.Text = "";
                            btn_pos_sel6.Text = "";
                            btn_pos_sel7.Text = "";
                            btn_pos_sel8.Text = "";

                            btn_pos_sel1.Enabled = false;
                            btn_pos_sel2.Enabled = false;
                            btn_pos_sel3.Enabled = false;
                            btn_pos_sel4.Enabled = false;
                            btn_pos_sel5.Enabled = false;
                            btn_pos_sel6.Enabled = false;
                            btn_pos_sel7.Enabled = false;
                            btn_pos_sel8.Enabled = false;

                            txt_solution.Enabled = false;
                            txt_score.Enabled = false;

                    }
                    else
                    {

                            audio_array[step_number, 1] = audio_number;

                            step_number++;

                            correct_answer_cnt++;

                            if (cBox_difficulty.Text == "Beginner")
                            {

                                btn_pos_sel1.Text = "1";
                                btn_pos_sel2.Text = "2";
                                btn_pos_sel3.Text = "3";
                                btn_pos_sel4.Text = "4";
                                btn_pos_sel5.Text = "5";
                                btn_pos_sel6.Text = "6";
                                btn_pos_sel7.Text = "7";
                                btn_pos_sel8.Text = "8";

                                paths = Directory.GetFiles(exe_location + @"\Audio_files\Beginner\Audio_" + audio_number.ToString());

                                word_number = paths.Length;                                   //we count, how many files we have read in


                                Randomize_buttons_beginner();                                            //we randomize the button distribution in a sequence - first word, first buttons

                            }
                            else {

                                btn_pos_sel1.Enabled = true;
                                btn_pos_sel2.Enabled = true;
                                btn_pos_sel3.Enabled = true;
                                btn_pos_sel4.Enabled = true;
                                btn_pos_sel5.Enabled = true;
                                btn_pos_sel6.Enabled = true;
                                btn_pos_sel7.Enabled = true;
                                btn_pos_sel8.Enabled = true;

                                pos_sel1 = 1;
                                pos_sel2 = 1;
                                pos_sel3 = 1;
                                pos_sel4 = 1;
                                pos_sel5 = 1;
                                pos_sel6 = 1;
                                pos_sel7 = 1;
                                pos_sel8 = 1;

                                for (int i = 0; i < 8; i++)
                                {

                                    pos_sel[i] = 1;

                                }

                                btn_pos_sel1.Text = "?";
                                btn_pos_sel2.Text = "?";
                                btn_pos_sel3.Text = "?";
                                btn_pos_sel4.Text = "?";
                                btn_pos_sel5.Text = "?";
                                btn_pos_sel6.Text = "?";
                                btn_pos_sel7.Text = "?";
                                btn_pos_sel8.Text = "?";

                                paths = Directory.GetFiles(exe_location + @"\Audio_files\Advanced\Audio_" + audio_number.ToString());

                                word_number = paths.Length;                                   //we count, how many files we have read in


                                Randomize_buttons_advanced();

                            }

                            if (word_number == 3)                                             //depending on the number of files, we adjust the number of buttons activated
                            {
                                first_word_btn1.Enabled = true;
                                first_word_btn2.Enabled = true;
                                first_word_btn1.BackColor = Color.White;
                                first_word_btn2.BackColor = Color.White;
                                second_word_correct = true;
                                third_word_correct = true;
                                fourth_word_correct = true;
                                fifth_word_correct = true;
                                sixth_word_correct = true;
                                seventh_word_correct = true;
                                eigtht_word_correct = true;
                                btn_pos_sel2.Text = "";
                                btn_pos_sel3.Text = "";
                                btn_pos_sel4.Text = "";
                                btn_pos_sel5.Text = "";
                                btn_pos_sel6.Text = "";
                                btn_pos_sel7.Text = "";
                                btn_pos_sel8.Text = "";

                            if (cBox_difficulty.Text == "Beginner")
                                {
                                    //do nothing
                                }
                                else {

                                    btn_pos_sel2.Enabled = false;
                                    btn_pos_sel3.Enabled = false;
                                    btn_pos_sel4.Enabled = false;
                                    btn_pos_sel5.Enabled = false;
                                    btn_pos_sel6.Enabled = false;
                                    btn_pos_sel7.Enabled = false;
                                    btn_pos_sel8.Enabled = false;

                                    for (int i = 1; i < 8; i++)
                                    {

                                        pos_sel[i] = 0;

                                    }

                            }

                            }
                            else if (word_number == 5)                                             //depending on the number of files, we adjust the number of buttons activated
                            {
                                first_word_btn1.Enabled = true;
                                first_word_btn2.Enabled = true;
                                second_word_btn1.Enabled = true;
                                second_word_btn2.Enabled = true;
                                first_word_btn1.BackColor = Color.White;
                                first_word_btn2.BackColor = Color.White;
                                second_word_btn1.BackColor = Color.White;
                                second_word_btn2.BackColor = Color.White;
                                third_word_correct = true;
                                fourth_word_correct = true;
                                fifth_word_correct = true;
                                sixth_word_correct = true;
                                seventh_word_correct = true;
                                eigtht_word_correct = true;
                                btn_pos_sel3.Text = "";
                                btn_pos_sel4.Text = "";
                                btn_pos_sel5.Text = "";
                                btn_pos_sel6.Text = "";
                                btn_pos_sel7.Text = "";
                                btn_pos_sel8.Text = "";

                                if (cBox_difficulty.Text == "Beginner")
                                {
                                    //do nothing
                                }
                                else
                                {


                                    btn_pos_sel3.Enabled = false;
                                    btn_pos_sel4.Enabled = false;
                                    btn_pos_sel5.Enabled = false;
                                    btn_pos_sel6.Enabled = false;
                                    btn_pos_sel7.Enabled = false;
                                    btn_pos_sel8.Enabled = false;

                                    for (int i = 2; i < 8; i++)
                                    {

                                        pos_sel[i] = 0;

                                    }

                            }

                            }
                            else if (word_number == 7)
                            {
                                first_word_btn1.Enabled = true;
                                first_word_btn2.Enabled = true;
                                second_word_btn1.Enabled = true;
                                second_word_btn2.Enabled = true;
                                third_word_btn1.Enabled = true;
                                third_word_btn2.Enabled = true;
                                first_word_btn1.BackColor = Color.White;
                                first_word_btn2.BackColor = Color.White;
                                second_word_btn1.BackColor = Color.White;
                                second_word_btn2.BackColor = Color.White;
                                third_word_btn1.BackColor = Color.White;
                                third_word_btn2.BackColor = Color.White;
                                fourth_word_correct = true;
                                fifth_word_correct = true;
                                sixth_word_correct = true;
                                seventh_word_correct = true;
                                eigtht_word_correct = true;
                                btn_pos_sel4.Text = "";
                                btn_pos_sel5.Text = "";
                                btn_pos_sel6.Text = "";
                                btn_pos_sel7.Text = "";
                                btn_pos_sel8.Text = "";

                                if (cBox_difficulty.Text == "Beginner")
                                {
                                    //do nothing
                                }
                                else
                                {


                                    btn_pos_sel4.Enabled = false;
                                    btn_pos_sel5.Enabled = false;
                                    btn_pos_sel6.Enabled = false;
                                    btn_pos_sel7.Enabled = false;
                                    btn_pos_sel8.Enabled = false;

                                    for (int i = 3; i < 8; i++)
                                    {

                                        pos_sel[i] = 0;

                                    }

                            }

                            }
                            else if (word_number == 9)
                            {
                                first_word_btn1.Enabled = true;
                                first_word_btn2.Enabled = true;
                                second_word_btn1.Enabled = true;
                                second_word_btn2.Enabled = true;
                                third_word_btn1.Enabled = true;
                                third_word_btn2.Enabled = true;
                                fourth_word_btn1.Enabled = true;
                                fourth_word_btn2.Enabled = true;
                                first_word_btn1.BackColor = Color.White;
                                first_word_btn2.BackColor = Color.White;
                                second_word_btn1.BackColor = Color.White;
                                second_word_btn2.BackColor = Color.White;
                                third_word_btn1.BackColor = Color.White;
                                third_word_btn2.BackColor = Color.White;
                                fourth_word_btn1.BackColor = Color.White;
                                fourth_word_btn2.BackColor = Color.White;
                                fifth_word_correct = true;
                                sixth_word_correct = true;
                                seventh_word_correct = true;
                                eigtht_word_correct = true;
                                btn_pos_sel5.Text = "";
                                btn_pos_sel6.Text = "";
                                btn_pos_sel7.Text = "";
                                btn_pos_sel8.Text = "";

                                if (cBox_difficulty.Text == "Beginner")
                                {
                                    //do nothing
                                }
                                else
                                {


                                    btn_pos_sel5.Enabled = false;
                                    btn_pos_sel6.Enabled = false;
                                    btn_pos_sel7.Enabled = false;
                                    btn_pos_sel8.Enabled = false;

                                    for (int i = 4; i < 8; i++)
                                    {

                                        pos_sel[i] = 0;

                                    }

                            }

                            }
                            else if (word_number == 11)
                            {
                                first_word_btn1.Enabled = true;
                                first_word_btn2.Enabled = true;
                                second_word_btn1.Enabled = true;
                                second_word_btn2.Enabled = true;
                                third_word_btn1.Enabled = true;
                                third_word_btn2.Enabled = true;
                                fourth_word_btn1.Enabled = true;
                                fourth_word_btn2.Enabled = true;
                                fifth_word_btn1.Enabled = true;
                                fifth_word_btn2.Enabled = true;
                                first_word_btn1.BackColor = Color.White;
                                first_word_btn2.BackColor = Color.White;
                                second_word_btn1.BackColor = Color.White;
                                second_word_btn2.BackColor = Color.White;
                                third_word_btn1.BackColor = Color.White;
                                third_word_btn2.BackColor = Color.White;
                                fourth_word_btn1.BackColor = Color.White;
                                fourth_word_btn2.BackColor = Color.White;
                                fifth_word_btn1.BackColor = Color.White;
                                fifth_word_btn2.BackColor = Color.White;
                                sixth_word_correct = true;
                                seventh_word_correct = true;
                                eigtht_word_correct = true;
                                btn_pos_sel6.Text = "";
                                btn_pos_sel7.Text = "";
                                btn_pos_sel8.Text = "";

                                if (cBox_difficulty.Text == "Beginner")
                                {
                                    //do nothing
                                }
                                else
                                {


                                    btn_pos_sel6.Enabled = false;
                                    btn_pos_sel7.Enabled = false;
                                    btn_pos_sel8.Enabled = false;

                                    for (int i = 5; i < 8; i++)
                                    {

                                        pos_sel[i] = 0;

                                    }
                            }
                            }
                            else if (word_number == 13)
                            {
                                first_word_btn1.Enabled = true;
                                first_word_btn2.Enabled = true;
                                second_word_btn1.Enabled = true;
                                second_word_btn2.Enabled = true;
                                third_word_btn1.Enabled = true;
                                third_word_btn2.Enabled = true;
                                fourth_word_btn1.Enabled = true;
                                fourth_word_btn2.Enabled = true;
                                fifth_word_btn1.Enabled = true;
                                fifth_word_btn2.Enabled = true;
                                sixth_word_btn1.Enabled = true;
                                sixth_word_btn2.Enabled = true;
                                first_word_btn1.BackColor = Color.White;
                                first_word_btn2.BackColor = Color.White;
                                second_word_btn1.BackColor = Color.White;
                                second_word_btn2.BackColor = Color.White;
                                third_word_btn1.BackColor = Color.White;
                                third_word_btn2.BackColor = Color.White;
                                fourth_word_btn1.BackColor = Color.White;
                                fourth_word_btn2.BackColor = Color.White;
                                fifth_word_btn1.BackColor = Color.White;
                                fifth_word_btn2.BackColor = Color.White;
                                sixth_word_btn1.BackColor = Color.White;
                                sixth_word_btn2.BackColor = Color.White;
                                seventh_word_correct = true;
                                eigtht_word_correct = true;
                                btn_pos_sel7.Text = "";
                                btn_pos_sel8.Text = "";
                                if (cBox_difficulty.Text == "Beginner")
                                {
                                    //do nothing
                                }
                                else
                                {

                                    btn_pos_sel7.Enabled = false;
                                    btn_pos_sel8.Enabled = false;

                                    for (int i = 6; i < 8; i++)
                                    {

                                        pos_sel[i] = 0;

                                    }

                            }

                            }
                            else if (word_number == 15)
                            {
                                first_word_btn1.Enabled = true;
                                first_word_btn2.Enabled = true;
                                second_word_btn1.Enabled = true;
                                second_word_btn2.Enabled = true;
                                third_word_btn1.Enabled = true;
                                third_word_btn2.Enabled = true;
                                fourth_word_btn1.Enabled = true;
                                fourth_word_btn2.Enabled = true;
                                fifth_word_btn1.Enabled = true;
                                fifth_word_btn2.Enabled = true;
                                sixth_word_btn1.Enabled = true;
                                sixth_word_btn2.Enabled = true;
                                seventh_word_btn1.Enabled = true;
                                seventh_word_btn2.Enabled = true;
                                first_word_btn1.BackColor = Color.White;
                                first_word_btn2.BackColor = Color.White;
                                second_word_btn1.BackColor = Color.White;
                                second_word_btn2.BackColor = Color.White;
                                third_word_btn1.BackColor = Color.White;
                                third_word_btn2.BackColor = Color.White;
                                fourth_word_btn1.BackColor = Color.White;
                                fourth_word_btn2.BackColor = Color.White;
                                fifth_word_btn1.BackColor = Color.White;
                                fifth_word_btn2.BackColor = Color.White;
                                sixth_word_btn1.BackColor = Color.White;
                                sixth_word_btn2.BackColor = Color.White;
                                seventh_word_btn1.BackColor = Color.White;
                                seventh_word_btn2.BackColor = Color.White;
                                eigtht_word_correct = true;
                                btn_pos_sel8.Text = "";

                                if (cBox_difficulty.Text == "Beginner")
                                {
                                    //do nothing
                                }
                                else
                                {

                                    btn_pos_sel8.Enabled = false;
                                    pos_sel[7] = 0;
                                }

                            }
                            else if (word_number == 17)
                            {
                                first_word_btn1.Enabled = true;
                                first_word_btn2.Enabled = true;
                                second_word_btn1.Enabled = true;
                                second_word_btn2.Enabled = true;
                                third_word_btn1.Enabled = true;
                                third_word_btn2.Enabled = true;
                                fourth_word_btn1.Enabled = true;
                                fourth_word_btn2.Enabled = true;
                                fifth_word_btn1.Enabled = true;
                                fifth_word_btn2.Enabled = true;
                                sixth_word_btn1.Enabled = true;
                                sixth_word_btn2.Enabled = true;
                                seventh_word_btn1.Enabled = true;
                                seventh_word_btn2.Enabled = true;
                                eighth_word_btn1.Enabled = true;
                                eighth_word_btn2.Enabled = true;
                                first_word_btn1.BackColor = Color.White;
                                first_word_btn2.BackColor = Color.White;
                                second_word_btn1.BackColor = Color.White;
                                second_word_btn2.BackColor = Color.White;
                                third_word_btn1.BackColor = Color.White;
                                third_word_btn2.BackColor = Color.White;
                                fourth_word_btn1.BackColor = Color.White;
                                fourth_word_btn2.BackColor = Color.White;
                                fifth_word_btn1.BackColor = Color.White;
                                fifth_word_btn2.BackColor = Color.White;
                                sixth_word_btn1.BackColor = Color.White;
                                sixth_word_btn2.BackColor = Color.White;
                                seventh_word_btn1.BackColor = Color.White;
                                seventh_word_btn2.BackColor = Color.White;
                                eighth_word_btn1.BackColor = Color.White;
                                eighth_word_btn2.BackColor = Color.White;

                            }

                            btn_open.Text = "Validate";

                            btn_solution.Enabled = true;

                            if (cBox_difficulty.Text == "Beginner")
                            {
                                //do nothing
                            }
                            else
                            {

                                btn_solution.Text = "Aid 0/3";
                                btn_pass.Enabled = true;
                                btn_pass.Text = "Pass";

                            }

                    }

                 }

            } else if((btn_open.Text == "Validate")|(btn_open.Text == "Try again"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {

                    if ((first_word_correct == true) && (second_word_correct == true) && (third_word_correct == true) && (fourth_word_correct == true) && (fifth_word_correct == true) && (sixth_word_correct == true) && (seventh_word_correct == true) && (eigtht_word_correct == true))
                    {

                        first_word_btn1.Enabled = false;
                        first_word_btn2.Enabled = false;
                        second_word_btn1.Enabled = false;
                        second_word_btn2.Enabled = false;
                        third_word_btn1.Enabled = false;
                        third_word_btn2.Enabled = false;
                        fourth_word_btn1.Enabled = false;
                        fourth_word_btn2.Enabled = false;
                        fifth_word_btn1.Enabled = false;
                        fifth_word_btn2.Enabled = false;
                        sixth_word_btn1.Enabled = false;
                        sixth_word_btn2.Enabled = false;
                        seventh_word_btn1.Enabled = false;
                        seventh_word_btn2.Enabled = false;
                        eighth_word_btn1.Enabled = false;
                        eighth_word_btn2.Enabled = false;

                        btn_solution.Enabled = false;
                        btn_pass.Enabled = false;

                        first_word_btn1.BackColor = Color.LightGray;
                        first_word_btn2.BackColor = Color.LightGray;
                        second_word_btn1.BackColor = Color.LightGray;
                        second_word_btn2.BackColor = Color.LightGray;
                        third_word_btn1.BackColor = Color.LightGray;
                        third_word_btn2.BackColor = Color.LightGray;
                        fourth_word_btn1.BackColor = Color.LightGray;
                        fourth_word_btn2.BackColor = Color.LightGray;
                        fifth_word_btn1.BackColor = Color.LightGray;
                        fifth_word_btn2.BackColor = Color.LightGray;
                        sixth_word_btn1.BackColor = Color.LightGray;
                        sixth_word_btn2.BackColor = Color.LightGray;
                        seventh_word_btn1.BackColor = Color.LightGray;
                        seventh_word_btn2.BackColor = Color.LightGray;
                        eighth_word_btn1.BackColor = Color.LightGray;
                        eighth_word_btn2.BackColor = Color.LightGray;


                        btn_pos_sel1.Text = "";
                        btn_pos_sel2.Text = "";
                        btn_pos_sel3.Text = "";
                        btn_pos_sel4.Text = "";
                        btn_pos_sel5.Text = "";
                        btn_pos_sel6.Text = "";
                        btn_pos_sel7.Text = "";
                        btn_pos_sel8.Text = "";

                        btn_pos_sel1.Enabled = false;
                        btn_pos_sel2.Enabled = false;
                        btn_pos_sel3.Enabled = false;
                        btn_pos_sel4.Enabled = false;
                        btn_pos_sel5.Enabled = false;
                        btn_pos_sel6.Enabled = false;
                        btn_pos_sel7.Enabled = false;
                        btn_pos_sel8.Enabled = false;

                        first_word_correct = false;
                        second_word_correct = false;
                        third_word_correct = false;
                        fourth_word_correct = false;
                        fifth_word_correct = false;
                        sixth_word_correct = false;
                        seventh_word_correct = false;
                        eigtht_word_correct = false;

                        first_select_correct = false;
                        second_select_correct = false;
                        third_select_correct = false;
                        fourth_select_correct = false;
                        fifth_select_correct = false;
                        sixth_select_correct = false;
                        seventh_select_correct = false;
                        eigtht_select_correct = false;

                        txt_score.Text = step_number.ToString() + "/" + number_of_tests.ToString(); ;
                        btn_open.Text = "Next";

                        txt_solution.Text = solutions[audio_number - 1];

                        media_player.Ctlcontrols.stop();
                        media_player.URL = paths[(word_number - 1)];                         //we play back the solution file
                        media_player.Ctlcontrols.play();

                    }
                    else
                    {

                        btn_open.Text = "Try again";

                        media_player.currentPlaylist = WrongAnswerPlaylist;

                        media_player.currentPlaylist.clear();

                        media_player.Ctlcontrols.stop();

                        IWMPMedia media_buf;

                        if (first_word_correct == true)
                        {
                            media_buf = media_player.newMedia(paths[0]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }
                        else
                        {
                            media_buf = media_player.newMedia(paths[1]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }

                        if (word_number == 3)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        if (second_word_correct == true)
                        {
                            media_buf = media_player.newMedia(paths[2]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }
                        else
                        {
                            media_buf = media_player.newMedia(paths[3]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }

                        if (word_number == 5)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        if (third_word_correct == true)
                        {
                            media_buf = media_player.newMedia(paths[4]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }
                        else
                        {
                            media_buf = media_player.newMedia(paths[5]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }

                        if (word_number == 7)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        if (fourth_word_correct == true)
                        {
                            media_buf = media_player.newMedia(paths[6]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }
                        else
                        {
                            media_buf = media_player.newMedia(paths[7]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }

                        if (word_number == 9)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        if (fifth_word_correct == true)
                        {
                            media_buf = media_player.newMedia(paths[8]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }
                        else
                        {
                            media_buf = media_player.newMedia(paths[9]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }

                        if (word_number == 11)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        if (sixth_word_correct == true)
                        {
                            media_buf = media_player.newMedia(paths[10]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }
                        else
                        {
                            media_buf = media_player.newMedia(paths[11]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }

                        if (word_number == 13)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        if (seventh_word_correct == true)
                        {
                            media_buf = media_player.newMedia(paths[12]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }
                        else
                        {
                            media_buf = media_player.newMedia(paths[13]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }

                        if (word_number == 15)
                        {
                            return;                                   //we break execution so we won't check for more words

                        }

                        if (eigtht_word_correct == true)
                        {
                            media_buf = media_player.newMedia(paths[14]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }
                        else
                        {
                            media_buf = media_player.newMedia(paths[15]);
                            WrongAnswerPlaylist.appendItem(media_buf);

                        }

                    }

                }
                else {

                    //we check if the proper selection is paired with the proper position
                    extract_position_advanced();

                    //if both the selection and the position is correct, we play back the soltuion just like in Beginner mode
                    if ((first_word_correct == true) && (second_word_correct == true) && (third_word_correct == true) && (fourth_word_correct == true) && (fifth_word_correct == true) && (sixth_word_correct == true) && (seventh_word_correct == true) && (eigtht_word_correct == true))
                    {

                        first_word_btn1.Enabled = false;
                        first_word_btn2.Enabled = false;
                        second_word_btn1.Enabled = false;
                        second_word_btn2.Enabled = false;
                        third_word_btn1.Enabled = false;
                        third_word_btn2.Enabled = false;
                        fourth_word_btn1.Enabled = false;
                        fourth_word_btn2.Enabled = false;
                        fifth_word_btn1.Enabled = false;
                        fifth_word_btn2.Enabled = false;
                        sixth_word_btn1.Enabled = false;
                        sixth_word_btn2.Enabled = false;
                        seventh_word_btn1.Enabled = false;
                        seventh_word_btn2.Enabled = false;
                        eighth_word_btn1.Enabled = false;
                        eighth_word_btn2.Enabled = false;

                        btn_solution.Enabled = false;
                        btn_pass.Enabled = false;

                        first_word_btn1.BackColor = Color.LightGray;
                        first_word_btn2.BackColor = Color.LightGray;
                        second_word_btn1.BackColor = Color.LightGray;
                        second_word_btn2.BackColor = Color.LightGray;
                        third_word_btn1.BackColor = Color.LightGray;
                        third_word_btn2.BackColor = Color.LightGray;
                        fourth_word_btn1.BackColor = Color.LightGray;
                        fourth_word_btn2.BackColor = Color.LightGray;
                        fifth_word_btn1.BackColor = Color.LightGray;
                        fifth_word_btn2.BackColor = Color.LightGray;
                        sixth_word_btn1.BackColor = Color.LightGray;
                        sixth_word_btn2.BackColor = Color.LightGray;
                        seventh_word_btn1.BackColor = Color.LightGray;
                        seventh_word_btn2.BackColor = Color.LightGray;
                        eighth_word_btn1.BackColor = Color.LightGray;
                        eighth_word_btn2.BackColor = Color.LightGray;

                        btn_pos_sel1.Text = "";
                        btn_pos_sel2.Text = "";
                        btn_pos_sel3.Text = "";
                        btn_pos_sel4.Text = "";
                        btn_pos_sel5.Text = "";
                        btn_pos_sel6.Text = "";
                        btn_pos_sel7.Text = "";
                        btn_pos_sel8.Text = "";

                        btn_pos_sel1.Enabled = false;
                        btn_pos_sel2.Enabled = false;
                        btn_pos_sel3.Enabled = false;
                        btn_pos_sel4.Enabled = false;
                        btn_pos_sel5.Enabled = false;
                        btn_pos_sel6.Enabled = false;
                        btn_pos_sel7.Enabled = false;
                        btn_pos_sel8.Enabled = false;

                        snippets[0].word_pos = "0";
                        snippets[0].path = "Lorem ipsum";
                        snippets[1].word_pos = "0";
                        snippets[1].path = "Lorem ipsum";
                        snippets[2].word_pos = "0";
                        snippets[2].path = "Lorem ipsum";
                        snippets[3].word_pos = "0";
                        snippets[3].path = "Lorem ipsum";
                        snippets[4].word_pos = "0";
                        snippets[4].path = "Lorem ipsum";
                        snippets[5].word_pos = "0";
                        snippets[5].path = "Lorem ipsum";
                        snippets[6].word_pos = "0";
                        snippets[6].path = "Lorem ipsum";
                        snippets[7].word_pos = "0";
                        snippets[7].path = "Lorem ipsum";

                        txt_solution.Text = solutions[audio_number - 1];

                        media_player.Ctlcontrols.stop();
                        media_player.URL = paths[(word_number - 1)];                         //we play back the solution file
                        media_player.Ctlcontrols.play();

                        first_word_correct = false;
                        second_word_correct = false;
                        third_word_correct = false;
                        fourth_word_correct = false;
                        fifth_word_correct = false;
                        sixth_word_correct = false;
                        seventh_word_correct = false;
                        eigtht_word_correct = false;

                        first_select_correct = false;
                        second_select_correct = false;
                        third_select_correct = false;
                        fourth_select_correct = false;
                        fifth_select_correct = false;
                        sixth_select_correct = false;
                        seventh_select_correct = false;
                        eigtht_select_correct = false;

                        aid_cnt = 0;

                        txt_score.Text = correct_answer_cnt.ToString() + "/" + number_of_tests.ToString();
                        btn_open.Text = "Next";

                    }
                    else //if either the selection or the position is wrong, we play back according to what was selected
                    {

                        btn_open.Text = "Try again";

                        media_player.currentPlaylist = WrongAnswerPlaylist;

                        media_player.currentPlaylist.clear();

                        media_player.Ctlcontrols.stop();

                        txt_solution.Text = "";

                        if ((btn_pos_sel1.Text == "?") |
                            (btn_pos_sel2.Text == "?") |
                            (btn_pos_sel3.Text == "?") |
                            (btn_pos_sel4.Text == "?") |
                            (btn_pos_sel5.Text == "?") |
                            (btn_pos_sel6.Text == "?") |
                            (btn_pos_sel7.Text == "?") |
                            (btn_pos_sel8.Text == "?")) {

                            txt_solution.Text = "Select a position for all snippets!";
                            return;

                        }

                        if (word_number > 3) {                                              //if we have more than 1 word but less than 8

                            for (int j = 0; j < (((word_number - 1) / 2) - 1); j++) {                         //we start from the first element and go until the one before the last

                                for (int i = 1; i < (((word_number - 1) / 2) - j); i++) {                     //we do relative stepping until the last element

                                    if (pos_sel[j] == pos_sel[j + i]) {

                                        txt_solution.Text = "Select a different position for all snippets!";
                                        return;

                                    };

                                }

                            }

                        } else { 
                        
                            //do nothing
                        
                        }

                        //here we need to reconstruct the playlist according to the selection we have done

                        IWMPMedia media_buf;


                        for (int i = 0; i < 8; i++) {


                            if (snippets[i].word_pos == "1")                        //we go through the snippet info and look for "1" in the word position
                            {
                                media_buf = media_player.newMedia(snippets[i].path);
                                WrongAnswerPlaylist.appendItem(media_buf);
                                break;

                            }
                            else { 
                            
                                //do nothing
                            
                            }

                        }

                        if (word_number == 3)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        for (int i = 0; i < 8; i++)
                        {

                            if (snippets[i].word_pos == "2")                        //we go through the snippet info and look for "1" in the word position
                            {

                                media_buf = media_player.newMedia(snippets[i].path);
                                WrongAnswerPlaylist.appendItem(media_buf);
                                break;

                            }
                            else
                            {

                                //do nothing

                            }

                        }

                        if (word_number == 5)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        for (int i = 0; i < 8; i++)
                        {

                            if (snippets[i].word_pos == "3")                        //we go through the snippet info and look for "1" in the word position
                            {

                                media_buf = media_player.newMedia(snippets[i].path);
                                WrongAnswerPlaylist.appendItem(media_buf);
                                break;

                            }
                            else
                            {

                                //do nothing

                            }

                        }

                        if (word_number == 7)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        for (int i = 0; i < 8; i++)
                        {

                            if (snippets[i].word_pos == "4")                        //we go through the snippet info and look for "1" in the word position
                            {

                                media_buf = media_player.newMedia(snippets[i].path);
                                WrongAnswerPlaylist.appendItem(media_buf);
                                break;

                            }
                            else
                            {

                                //do nothing

                            }

                        }

                        if (word_number == 9)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        for (int i = 0; i < 8; i++)
                        {

                            if (snippets[i].word_pos == "5")                        //we go through the snippet info and look for "1" in the word position
                            {

                                media_buf = media_player.newMedia(snippets[i].path);
                                WrongAnswerPlaylist.appendItem(media_buf);
                                break;

                            }
                            else
                            {

                                //do nothing

                            }

                        }

                        if (word_number == 11)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        for (int i = 0; i < 8; i++)
                        {

                            if (snippets[i].word_pos == "6")                        //we go through the snippet info and look for "1" in the word position
                            {

                                media_buf = media_player.newMedia(snippets[i].path);
                                WrongAnswerPlaylist.appendItem(media_buf);
                                break;

                            }
                            else
                            {

                                //do nothing

                            }

                        }

                        if (word_number == 13)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        for (int i = 0; i < 8; i++)
                        {

                            if (snippets[i].word_pos == "7")                        //we go through the snippet info and look for "1" in the word position
                            {

                                media_buf = media_player.newMedia(snippets[i].path);
                                WrongAnswerPlaylist.appendItem(media_buf);
                                break;

                            }
                            else
                            {

                                //do nothing

                            }

                        }

                        if (word_number == 15)
                        {

                            return;                                   //we break execution so we won't check for more words

                        }

                        for (int i = 0; i < 8; i++)
                        {

                            if (snippets[i].word_pos == "8")                        //we go through the snippet info and look for "1" in the word position
                            {

                                media_buf = media_player.newMedia(snippets[i].path);
                                WrongAnswerPlaylist.appendItem(media_buf);
                                break;

                            }
                            else
                            {

                                //do nothing

                            }

                        }

                    }

                }

            }

   
        }

        private void first_word_btn1_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[first_word_btn1_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            first_word_btn1.BackColor = Color.CadetBlue;
            first_word_btn2.BackColor = Color.White;
            first_word_select = first_word_btn1_val;

            if (paths[first_word_btn1_val].Substring(paths[first_word_btn1_val].Length - 6).Contains("A") )             //we check the path's last 6 letters for the letter "A" indicating a correct snippet
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    first_word_correct = true;

                }
                else
                {

                    first_select_correct = true;                                                //we have selected the right word from the snippets
                    snippets[0].path = paths[first_word_btn1_val];

                }

            }
            else
            {

                first_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {
                    first_select_correct = false;
                    snippets[0].path = paths[first_word_btn1_val];

                }

            }
        }

        private void first_word_btn2_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[first_word_btn2_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            first_word_btn1.BackColor = Color.White;
            first_word_btn2.BackColor = Color.CadetBlue;
            first_word_select = first_word_btn2_val;

            if (paths[first_word_btn2_val].Substring(paths[first_word_btn2_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    first_word_correct = true;

                }
                else
                {

                    first_select_correct = true;                                                //we have selected the right word from the snippets
                    snippets[0].path = paths[first_word_btn2_val];


                }

            }
            else
            {

                first_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    first_select_correct = false;
                    snippets[0].path = paths[first_word_btn2_val];

                }

            }
        }

        private void second_word_btn1_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[second_word_btn1_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            second_word_btn1.BackColor = Color.CadetBlue;
            second_word_btn2.BackColor = Color.White;
            second_word_select = second_word_btn1_val;

            if (paths[second_word_btn1_val].Substring(paths[second_word_btn1_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    second_word_correct = true;

                }
                else { 
                
                    second_select_correct = true;
                    snippets[1].path = paths[second_word_btn1_val];

                }

            }
            else
            {

                second_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    second_select_correct = false;
                    snippets[1].path = paths[second_word_btn1_val];

                }

            }
        }

        private void second_word_btn2_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[second_word_btn2_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            second_word_btn1.BackColor = Color.White;
            second_word_btn2.BackColor = Color.CadetBlue;
            second_word_select = second_word_btn2_val;

            if (paths[second_word_btn2_val].Substring(paths[second_word_btn2_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    second_word_correct = true;

                }
                else
                {

                    second_select_correct = true;
                    snippets[1].path = paths[second_word_btn2_val];

                }

            }
            else
            {
                second_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    second_select_correct = false;
                    snippets[1].path = paths[second_word_btn2_val];

                }

            }
        }

        private void third_word_btn1_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[third_word_btn1_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            third_word_btn1.BackColor = Color.CadetBlue;
            third_word_btn2.BackColor = Color.White;
            third_word_select = third_word_btn1_val;

            if (paths[third_word_btn1_val].Substring(paths[third_word_btn1_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    third_word_correct = true;

                }
                else
                {

                    third_select_correct = true;
                    snippets[2].path = paths[third_word_btn1_val];

                }

            }
            else
            {
                third_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    third_select_correct = false;
                    snippets[2].path = paths[third_word_btn1_val];

                }

            }
        }

        private void third_word_btn2_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[third_word_btn2_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            third_word_btn1.BackColor = Color.White;
            third_word_btn2.BackColor = Color.CadetBlue;
            third_word_select = third_word_btn2_val;

            if (paths[third_word_btn2_val].Substring(paths[third_word_btn2_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    third_word_correct = true;

                }
                else
                {

                    third_select_correct = true;
                    snippets[2].path = paths[third_word_btn2_val];

                }

            }
            else
            {
                third_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    third_select_correct = false;
                    snippets[2].path = paths[third_word_btn2_val];

                }

            }
        }

        private void fourth_word_btn1_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[fourth_word_btn1_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            fourth_word_btn1.BackColor = Color.CadetBlue;
            fourth_word_btn2.BackColor = Color.White;
            fourth_word_select = fourth_word_btn1_val;

            if (paths[fourth_word_btn1_val].Substring(paths[fourth_word_btn1_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    fourth_word_correct = true;

                }
                else
                {

                    fourth_select_correct = true;
                    snippets[3].path = paths[fourth_word_btn1_val];

                }

            }
            else
            {

                fourth_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothiung

                }
                else
                {

                    fourth_select_correct = false;
                    snippets[3].path = paths[fourth_word_btn1_val];

                }

            }
        }

        private void fourth_word_btn2_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[fourth_word_btn2_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            fourth_word_btn1.BackColor = Color.White;
            fourth_word_btn2.BackColor = Color.CadetBlue;
            fourth_word_select = fourth_word_btn2_val;

            if (paths[fourth_word_btn2_val].Substring(paths[fourth_word_btn2_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    fourth_word_correct = true;

                }
                else
                {

                    fourth_select_correct = true;
                    snippets[3].path = paths[fourth_word_btn2_val];

                }

            }
            else
            {

                fourth_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    fourth_select_correct = false;
                    snippets[3].path = paths[fourth_word_btn2_val];

                }

            }
        }

        private void fifth_word_btn1_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[fifth_word_btn1_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            fifth_word_btn1.BackColor = Color.CadetBlue;
            fifth_word_btn2.BackColor = Color.White;
            fifth_word_select = fifth_word_btn1_val;

            if (paths[fifth_word_btn1_val].Substring(paths[fifth_word_btn1_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    fifth_word_correct = true;

                }
                else
                {

                    fifth_select_correct = true;
                    snippets[4].path = paths[fifth_word_btn1_val];

                }

            }
            else
            {

                fifth_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                     //do nothing

                }
                else
                {

                    fifth_select_correct = false;
                    snippets[4].path = paths[fifth_word_btn1_val];

                }

            }
        }

        private void fifth_word_btn2_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[fifth_word_btn2_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            fifth_word_btn1.BackColor = Color.White;
            fifth_word_btn2.BackColor = Color.CadetBlue;
            fifth_word_select = fifth_word_btn2_val;

            if (paths[fifth_word_btn2_val].Substring(paths[fifth_word_btn2_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    fifth_word_correct = true;

                }
                else
                {

                    fifth_select_correct = true;
                    snippets[4].path = paths[fifth_word_btn2_val];

                }

            }
            else
            {

                fifth_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    fifth_select_correct = false;
                    snippets[4].path = paths[fifth_word_btn2_val];

                }

            }
        }

        private void sixth_word_btn1_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[sixth_word_btn1_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            sixth_word_btn1.BackColor = Color.CadetBlue;
            sixth_word_btn2.BackColor = Color.White;
            sixth_word_select = sixth_word_btn1_val;

            if (paths[sixth_word_btn1_val].Substring(paths[sixth_word_btn1_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    sixth_word_correct = true;

                }
                else
                {

                    sixth_select_correct = true;
                    snippets[5].path = paths[sixth_word_btn1_val];

                }

            }
            else
            {

                sixth_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    sixth_select_correct = false;
                    snippets[5].path = paths[sixth_word_btn1_val];

                }

            }
        }

        private void sixth_word_btn2_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[sixth_word_btn2_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            sixth_word_btn1.BackColor = Color.White;
            sixth_word_btn2.BackColor = Color.CadetBlue;
            sixth_word_select = sixth_word_btn2_val;

            if (paths[sixth_word_btn2_val].Substring(paths[sixth_word_btn2_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    sixth_word_correct = true;

                }
                else
                {

                    sixth_select_correct = true;
                    snippets[5].path = paths[sixth_word_btn2_val];

                }

            }
            else
            {

                sixth_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    sixth_select_correct = false;
                    snippets[5].path = paths[sixth_word_btn2_val];

                }

            }
        }

        private void seventh_word_btn1_Click(object sender, EventArgs e)
        {

            media_player.Ctlcontrols.stop();
            media_player.URL = paths[seventh_word_btn1_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            seventh_word_btn1.BackColor = Color.CadetBlue;
            seventh_word_btn2.BackColor = Color.White;
            seventh_word_select = seventh_word_btn1_val;

            if (paths[seventh_word_btn1_val].Substring(paths[seventh_word_btn1_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    seventh_word_correct = true;

                }
                else
                {

                    seventh_select_correct = true;
                    snippets[6].path = paths[seventh_word_btn1_val];

                }

            }
            else
            {

                seventh_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    seventh_select_correct = false;
                    snippets[6].path = paths[seventh_word_btn1_val];

                }

            }

        }

        private void seventh_word_btn2_Click(object sender, EventArgs e)
        {

            media_player.Ctlcontrols.stop();
            media_player.URL = paths[seventh_word_btn2_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            seventh_word_btn1.BackColor = Color.White;
            seventh_word_btn2.BackColor = Color.CadetBlue;
            seventh_word_select = seventh_word_btn2_val;

            if (paths[seventh_word_btn2_val].Substring(paths[seventh_word_btn2_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    seventh_word_correct = true;

                }
                else
                {

                    seventh_select_correct = true;
                    snippets[6].path = paths[seventh_word_btn2_val];

                }

            }
            else
            {

                seventh_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    seventh_select_correct = false;
                    snippets[6].path = paths[seventh_word_btn2_val];

                }

            }

        }

        private void eighth_word_btn1_Click(object sender, EventArgs e)
        {

            media_player.Ctlcontrols.stop();
            media_player.URL = paths[eigtht_word_btn1_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            eighth_word_btn1.BackColor = Color.CadetBlue;
            eighth_word_btn2.BackColor = Color.White;
            eigtht_word_select = eigtht_word_btn1_val;

            if (paths[eigtht_word_btn1_val].Substring(paths[eigtht_word_btn1_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    eigtht_word_correct = true;

                }
                else
                {

                    eigtht_select_correct = true;
                    snippets[7].path = paths[eigtht_word_btn1_val];

                }

            }
            else
            {

                eigtht_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    eigtht_select_correct = false;
                    snippets[7].path = paths[eigtht_word_btn1_val];

                }

            }

        }

        private void eighth_word_btn2_Click(object sender, EventArgs e)
        {

            media_player.Ctlcontrols.stop();
            media_player.URL = paths[eigtht_word_btn2_val];                         //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();
            eighth_word_btn1.BackColor = Color.White;
            eighth_word_btn2.BackColor = Color.CadetBlue;
            eigtht_word_select = eigtht_word_btn2_val;

            if (paths[eigtht_word_btn2_val].Substring(paths[eigtht_word_btn2_val].Length - 6).Contains("A"))
            {

                if (cBox_difficulty.Text == "Beginner")
                {
                    eigtht_word_correct = true;

                }
                else
                {

                    eigtht_select_correct = true;
                    snippets[7].path = paths[eigtht_word_btn2_val];

                }

            }
            else
            {

                eigtht_word_correct = false;

                if (cBox_difficulty.Text == "Beginner")
                {
                    //do nothing

                }
                else
                {

                    eigtht_select_correct = false;
                    snippets[7].path = paths[eigtht_word_btn2_val];

                }

            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //stuff to do when we close the app
            WrongAnswerPlaylist.clear();                                                //we wipe the playlist
        }
 

        private void Randomize_buttons_beginner() {
            //we randomly assign the snippet pairs to the button pairs
            //we do this in a sequence so first snippets will always go to the first buttons

            Random rnd_word = new Random();
            first_word_btn1_val = rnd_word.Next(0, 2);                           //we generate a random number between 0 and 1
            if (first_word_btn1_val == 0)                                        //we assign the values to the buttons depending on the random value we have generated above
            {

                first_word_btn2_val = 1;

            }
            else
            {

                first_word_btn2_val = 0;

            }

            second_word_btn1_val = rnd_word.Next(2, 4);                           //we generate a random number between 0 and 1
            if (second_word_btn1_val == 2)
            {

                second_word_btn2_val = 3;

            }
            else
            {

                second_word_btn2_val = 2;

            }

            third_word_btn1_val = rnd_word.Next(4, 6);                           //we generate a random number between 0 and 1
            if (third_word_btn1_val == 4)
            {

                third_word_btn2_val = 5;

            }
            else
            {

                third_word_btn2_val = 4;

            }

            fourth_word_btn1_val = rnd_word.Next(6, 8);                           //we generate a random number between 0 and 1
            if (fourth_word_btn1_val == 6)
            {

                fourth_word_btn2_val = 7;

            }
            else
            {

                fourth_word_btn2_val = 6;

            }

            fifth_word_btn1_val = rnd_word.Next(8, 10);                           //we generate a random number between 0 and 1
            if (fifth_word_btn1_val == 8)
            {

                fifth_word_btn2_val = 9;

            }
            else
            {

                fifth_word_btn2_val = 8;

            }

            sixth_word_btn1_val = rnd_word.Next(10, 12);                           //we generate a random number between 0 and 1
            if (sixth_word_btn1_val == 10)
            {

                sixth_word_btn2_val = 11;

            }
            else
            {

                sixth_word_btn2_val = 10;

            }


            seventh_word_btn1_val = rnd_word.Next(12, 14);                           //we generate a random number between 0 and 1
            if (seventh_word_btn1_val == 12)
            {

                seventh_word_btn2_val = 13;

            }
            else
            {

                seventh_word_btn2_val = 12;

            }


            eigtht_word_btn1_val = rnd_word.Next(14, 16);                           //we generate a random number between 0 and 1
            if (eigtht_word_btn1_val == 14)
            {

                eigtht_word_btn2_val = 15;

            }
            else
            {

                eigtht_word_btn2_val = 14;

            }

        }

        private void Randomize_buttons_advanced()
        {
            //we randomly assign the snippet pairs to the button pairs
            //first snippets may not go to the first buttons
            //here we should generate btn_val pairs randomly but never the same pair

            Random rnd_word = new Random();
            first_word_btn1_val = rnd_word.Next(0, (word_number - 1));                           //we generate a random number between 0 and 1
            if (first_word_btn1_val % 2 == 0)                                        //we assign the values to the buttons depending on the random value we have generated above
            {

                first_word_btn2_val = first_word_btn1_val + 1;                      //if the value was even, we select the odd

            }
            else
            {

                first_word_btn2_val = first_word_btn1_val - 1;                      //if it was odd, we select the even

            }

            if (word_number == 3) return;

            //------

            second_word_btn1_val = rnd_word.Next(0, (word_number - 1));                           //we generate a random number between 0 and 16

            while ((second_word_btn1_val == first_word_btn1_val) | (second_word_btn1_val == first_word_btn2_val)) {                  //we search until we have found a value outside the previous pair

                second_word_btn1_val = rnd_word.Next(0, (word_number - 1));

            };

            if (second_word_btn1_val % 2 == 0)
            {

                second_word_btn2_val = second_word_btn1_val + 1;

            }
            else
            {

                second_word_btn2_val = second_word_btn1_val - 1; ;

            }

            if (word_number == 5) return;

            //------

            third_word_btn1_val = rnd_word.Next(0, (word_number - 1));                           //we generate a random number between 0 and 1

            while ((third_word_btn1_val == first_word_btn1_val) | (third_word_btn1_val == first_word_btn2_val) |
                   (third_word_btn1_val == second_word_btn1_val) | (third_word_btn1_val == second_word_btn2_val)
                )
            {                  //we search until we have found a value outside the previous pair

                third_word_btn1_val = rnd_word.Next(0, (word_number - 1));

            };


            if (third_word_btn1_val % 2 == 0)
            {

                third_word_btn2_val = third_word_btn1_val + 1;

            }
            else
            {

                third_word_btn2_val = third_word_btn1_val - 1;

            }

            if (word_number == 7) return;

            //------

            fourth_word_btn1_val = rnd_word.Next(0, (word_number - 1));                           //we generate a random number between 0 and 1

            while ((fourth_word_btn1_val == first_word_btn1_val) | (fourth_word_btn1_val == first_word_btn2_val) |
                   (fourth_word_btn1_val == second_word_btn1_val) | (fourth_word_btn1_val == second_word_btn2_val) |
                   (fourth_word_btn1_val == third_word_btn1_val) | (fourth_word_btn1_val == third_word_btn2_val)
                )
            {                  //we search until we have found a value outside the previous pair

                fourth_word_btn1_val = rnd_word.Next(0, (word_number - 1));

            };


            if (fourth_word_btn1_val % 2 == 0)
            {

                fourth_word_btn2_val = fourth_word_btn1_val + 1;

            }
            else
            {

                fourth_word_btn2_val = fourth_word_btn1_val - 1;

            }

            if (word_number == 9) return;

            //------

            fifth_word_btn1_val = rnd_word.Next(0, (word_number - 1));                           //we generate a random number between 0 and 1

            while ((fifth_word_btn1_val == first_word_btn1_val) | (fifth_word_btn1_val == first_word_btn2_val) |
                   (fifth_word_btn1_val == second_word_btn1_val) | (fifth_word_btn1_val == second_word_btn2_val) |
                   (fifth_word_btn1_val == third_word_btn1_val) | (fifth_word_btn1_val == third_word_btn2_val) |
                   (fifth_word_btn1_val == fourth_word_btn1_val) | (fifth_word_btn1_val == fourth_word_btn2_val)
                )
            {                  //we search until we have found a value outside the previous pair

                fifth_word_btn1_val = rnd_word.Next(0, (word_number - 1));

            };


            if (fifth_word_btn1_val % 2 == 0)
            {

                fifth_word_btn2_val = fifth_word_btn1_val + 1;

            }
            else
            {

                fifth_word_btn2_val = fifth_word_btn1_val - 1;

            }

            if (word_number == 11) return;

            //------

            sixth_word_btn1_val = rnd_word.Next(0, (word_number - 1));                           //we generate a random number between 0 and 1

            while ((sixth_word_btn1_val == first_word_btn1_val) | (sixth_word_btn1_val == first_word_btn2_val) |
                   (sixth_word_btn1_val == second_word_btn1_val) | (sixth_word_btn1_val == second_word_btn2_val) |
                   (sixth_word_btn1_val == third_word_btn1_val) | (sixth_word_btn1_val == third_word_btn2_val) |
                   (sixth_word_btn1_val == fourth_word_btn1_val) | (sixth_word_btn1_val == fourth_word_btn2_val) |
                   (sixth_word_btn1_val == fifth_word_btn1_val) | (sixth_word_btn1_val == fifth_word_btn2_val)
                )
            {                  //we search until we have found a value outside the previous pair

                sixth_word_btn1_val = rnd_word.Next(0, (word_number - 1));

            };


            if (sixth_word_btn1_val % 2 == 0)
            {

                sixth_word_btn2_val = sixth_word_btn1_val + 1;

            }
            else
            {

                sixth_word_btn2_val = sixth_word_btn1_val - 1;

            }

            if (word_number == 13) return;

            //------

            seventh_word_btn1_val = rnd_word.Next(0, (word_number - 1));                           //we generate a random number between 0 and 1

            while ((seventh_word_btn1_val == first_word_btn1_val) | (seventh_word_btn1_val == first_word_btn2_val) |
                   (seventh_word_btn1_val == second_word_btn1_val) | (seventh_word_btn1_val == second_word_btn2_val) |
                   (seventh_word_btn1_val == third_word_btn1_val) | (seventh_word_btn1_val == third_word_btn2_val) |
                   (seventh_word_btn1_val == fourth_word_btn1_val) | (seventh_word_btn1_val == fourth_word_btn2_val) |
                   (seventh_word_btn1_val == fifth_word_btn1_val) | (seventh_word_btn1_val == fifth_word_btn2_val) |
                   (seventh_word_btn1_val == sixth_word_btn1_val) | (seventh_word_btn1_val == sixth_word_btn2_val)
                )
            {                  //we search until we have found a value outside the previous pair

                seventh_word_btn1_val = rnd_word.Next(0, (word_number - 1));

            };


            if (seventh_word_btn1_val % 2 == 0)
            {

                seventh_word_btn2_val = seventh_word_btn1_val + 1;

            }
            else
            {

                seventh_word_btn2_val = seventh_word_btn1_val - 1;

            }

            if (word_number == 15) return;

            //------

            eigtht_word_btn1_val = rnd_word.Next(0, (word_number - 1));                           //we generate a random number between 0 and 1

            while ((eigtht_word_btn1_val == first_word_btn1_val) | (eigtht_word_btn1_val == first_word_btn2_val) |
                   (eigtht_word_btn1_val == second_word_btn1_val) | (eigtht_word_btn1_val == second_word_btn2_val) |
                   (eigtht_word_btn1_val == third_word_btn1_val) | (eigtht_word_btn1_val == third_word_btn2_val) |
                   (eigtht_word_btn1_val == fourth_word_btn1_val) | (eigtht_word_btn1_val == fourth_word_btn2_val) |
                   (eigtht_word_btn1_val == fifth_word_btn1_val) | (eigtht_word_btn1_val == fifth_word_btn2_val) |
                   (eigtht_word_btn1_val == sixth_word_btn1_val) | (eigtht_word_btn1_val == sixth_word_btn2_val) |
                   (eigtht_word_btn1_val == seventh_word_btn1_val) | (eigtht_word_btn1_val == seventh_word_btn2_val)
                )
            {                  //we search until we have found a value outside the previous pair

                eigtht_word_btn1_val = rnd_word.Next(0, (word_number - 1));

            };


            if (eigtht_word_btn1_val % 2 == 0)
            {

                eigtht_word_btn2_val = eigtht_word_btn1_val + 1;

            }
            else
            {

                eigtht_word_btn2_val = eigtht_word_btn1_val - 1;

            }

            //------

        }

        private void extract_position_advanced() {

            ///we go through the given psoition values and etxratc them into an integer array

            if (first_select_correct == true)                   //if the right snippet is selected
                                                                //we check if it is placed in the right spot
                                                                //btn value for the words doesn't matter since the string section we check if the same for both
            {
                if (btn_pos_sel1.Text == (paths[first_word_btn1_val].Substring(paths[first_word_btn1_val].Length - 6)).Substring(0, 1))
                {     //check if the snippet number is the same as the snippet position

                    first_word_correct = true;                                              //we gave the right position

                }
                else
                {

                    first_word_correct = false;                                             //we didn't give the right position

                }

            }
            else
            {

                //do nothing

            }

            snippets[0].word_pos = btn_pos_sel1.Text;

//           snippet_pos_array[0] = int.Parse(txtbox_1.Text);                                   //we extract the position

            if (word_number == 3)
            {
                return;                                   //we break execution so we won't check for more words

            }

            if (second_select_correct == true)

            {
                if (btn_pos_sel2.Text == (paths[second_word_btn1_val].Substring(paths[second_word_btn1_val].Length - 6)).Substring(0, 1))
                {     //check if the snippet number is the same as the snippet position

                    second_word_correct = true;                                              //we gave the right position

                }
                else
                {

                    second_word_correct = false;                                             //we didn't give the right position

                }

            }
            else
            {

                //do nothing

            }

            snippets[1].word_pos = btn_pos_sel2.Text;

            if (word_number == 5)
            {
                return;                                   //we break execution so we won't check for more words

            }

            if (third_select_correct == true)                   //if the right snippet is selected
                                                                //we check if it is placed in the right spot
                                                                //btn value for the words doesn't matter since the string section we check if the same for both
            {
                if (btn_pos_sel3.Text == (paths[third_word_btn1_val].Substring(paths[third_word_btn1_val].Length - 6)).Substring(0, 1))
                {     //check if the snippet number is the same as the snippet position

                    third_word_correct = true;                                              //we gave the right position

                }
                else
                {

                    third_word_correct = false;                                             //we didn't give the right position

                }

            }
            else
            {

                //do nothing

            }

            snippets[2].word_pos = btn_pos_sel3.Text;

            if (word_number == 7)
            {
                return;                                   //we break execution so we won't check for more words

            }

            if (fourth_select_correct == true)                   //if the right snippet is selected
                                                                 //we check if it is placed in the right spot
                                                                 //btn value for the words doesn't matter since the string section we check if the same for both
            {
                if (btn_pos_sel4.Text == (paths[fourth_word_btn1_val].Substring(paths[fourth_word_btn1_val].Length - 6)).Substring(0, 1))
                {     //check if the snippet number is the same as the snippet position

                    fourth_word_correct = true;                                              //we gave the right position

                }
                else
                {

                    fourth_word_correct = false;                                             //we didn't give the right position

                }

            }
            else
            {

                //do nothing

            }

            snippets[3].word_pos = btn_pos_sel4.Text;

            if (word_number == 9)
            {
                return;                                   //we break execution so we won't check for more words

            }

            if (fifth_select_correct == true)                   //if the right snippet is selected
                                                                //we check if it is placed in the right spot
                                                                //btn value for the words doesn't matter since the string section we check if the same for both
            {
                if (btn_pos_sel5.Text == (paths[fifth_word_btn1_val].Substring(paths[fifth_word_btn1_val].Length - 6)).Substring(0, 1))
                {     //check if the snippet number is the same as the snippet position

                    fifth_word_correct = true;                                              //we gave the right position

                }
                else
                {

                    fifth_word_correct = false;                                             //we didn't give the right position

                }

            }
            else
            {

                //do nothing

            }

            snippets[4].word_pos = btn_pos_sel5.Text;

            if (word_number == 11)
            {
                return;                                   //we break execution so we won't check for more words

            }

            if (sixth_select_correct == true)                   //if the right snippet is selected
                                                                //we check if it is placed in the right spot
                                                                //btn value for the words doesn't matter since the string section we check if the same for both
            {
                if (btn_pos_sel6.Text == (paths[sixth_word_btn1_val].Substring(paths[sixth_word_btn1_val].Length - 6)).Substring(0, 1))
                {     //check if the snippet number is the same as the snippet position

                    sixth_word_correct = true;                                              //we gave the right position

                }
                else
                {

                    sixth_word_correct = false;                                             //we didn't give the right position

                }

            }
            else
            {

                //do nothing

            }

            snippets[5].word_pos = btn_pos_sel6.Text;

            if (word_number == 13)
            {
                return;                                   //we break execution so we won't check for more words

            }

            if (seventh_select_correct == true)                   //if the right snippet is selected
                                                                  //we check if it is placed in the right spot
                                                                  //btn value for the words doesn't matter since the string section we check if the same for both
            {
                if (btn_pos_sel7.Text == (paths[seventh_word_btn1_val].Substring(paths[seventh_word_btn1_val].Length - 6)).Substring(0, 1))
                {     //check if the snippet number is the same as the snippet position

                    seventh_word_correct = true;                                              //we gave the right position

                }
                else
                {

                    seventh_word_correct = false;                                             //we didn't give the right position

                }

            }
            else
            {

                //do nothing

            }

            snippets[6].word_pos = btn_pos_sel7.Text;

            if (word_number == 15)
            {
                return;                                   //we break execution so we won't check for more words

            }

            if (eigtht_select_correct == true)                   //if the right snippet is selected
                                                                 //we check if it is placed in the right spot
                                                                 //btn value for the words doesn't matter since the string section we check if the same for both
            {
                if (btn_pos_sel8.Text == (paths[eigtht_word_btn1_val].Substring(paths[eigtht_word_btn1_val].Length - 6)).Substring(0, 1))
                {     //check if the snippet number is the same as the snippet position

                    eigtht_word_correct = true;                                              //we gave the right position

                }
                else
                {

                    eigtht_word_correct = false;                                             //we didn't give the right position

                }

            }
            else
            {

                //do nothing

            }

            snippets[7].word_pos = btn_pos_sel8.Text;

        }

        private void media_player_PlaylistChange(object sender, AxWMPLib._WMPOCXEvents_PlaylistChangeEvent e)
        {

            if ((media_player.currentPlaylist.count == ((word_number - 1) / 2)) && (wrong_answer_published == false))       //we wait until the playlist is full
            {
                media_player.Ctlcontrols.play();                                                                               //and then play back
                wrong_answer_published = true;

            }
            else { 
            
                //do nothing

            }

        }

        private void btn_solution_Click(object sender, EventArgs e)
        {
            media_player.Ctlcontrols.stop();
            media_player.URL = paths[(word_number - 1)];                          //we select the URL form the paths which should be the same index as on the track list
            media_player.Ctlcontrols.play();

            if (cBox_difficulty.Text == "Beginner")
            {

                //do nothing

            }
            else {

                if (aid_cnt == 2)
                {

                    btn_solution.Enabled = false;                                       //if it is advanced mode, we have only one help allowed

                }
                else {

                    //do nothing

                }

                aid_cnt++;
                btn_solution.Text = "Aid " + aid_cnt.ToString() + "/3";

            }
        }

        private void btn_pos_sel1_Click(object sender, EventArgs e)
        {
            btn_pos_sel1.Text = pos_sel1.ToString();

            if (pos_sel1 == ((word_number - 1) / 2))
            {

                pos_sel1 = 1;

            }
            else {

                pos_sel1++;

            }

            pos_sel[0] = pos_sel1;

        }

        private void btn_pos_sel2_Click(object sender, EventArgs e)
        {
            btn_pos_sel2.Text = pos_sel2.ToString();

            if (pos_sel2 == ((word_number - 1) / 2))
            {

                pos_sel2 = 1;

            }
            else
            {

                pos_sel2++;

            }

            pos_sel[1] = pos_sel2;

        }

        private void btn_pos_sel3_Click(object sender, EventArgs e)
        {

            btn_pos_sel3.Text = pos_sel3.ToString();

            if (pos_sel3 == ((word_number - 1) / 2))
            {

                pos_sel3 = 1;

            }
            else
            {

                pos_sel3++;

            }

            pos_sel[2] = pos_sel3;


        }

        private void btn_pos_sel4_Click(object sender, EventArgs e)
        {

            btn_pos_sel4.Text = pos_sel4.ToString();

            if (pos_sel4 == ((word_number - 1) / 2))
            {

                pos_sel4 = 1;

            }
            else
            {

                pos_sel4++;

            }

            pos_sel[3] = pos_sel4;


        }

        private void btn_pos_sel5_Click(object sender, EventArgs e)
        {

            btn_pos_sel5.Text = pos_sel5.ToString();

            if (pos_sel5 == ((word_number - 1) / 2))
            {

                pos_sel5 = 1;

            }
            else
            {

                pos_sel5++;

            }

            pos_sel[4] = pos_sel5;


        }

        private void btn_pos_sel6_Click(object sender, EventArgs e)
        {
            
            btn_pos_sel6.Text = pos_sel6.ToString();

            if(pos_sel6 == ((word_number - 1) / 2))
            {

                pos_sel6 = 1;

            }
            else
            {

                pos_sel6++;

            }

            pos_sel[5] = pos_sel6;


        }

        private void btn_pos_sel7_Click(object sender, EventArgs e)
        {

            btn_pos_sel7.Text = pos_sel7.ToString();
            if (pos_sel7 == ((word_number - 1) / 2))
            {

                pos_sel7 = 1;

            }
            else
            {

                pos_sel7++;

            }

            pos_sel[6] = pos_sel7;

        }

        private void btn_pos_sel8_Click(object sender, EventArgs e)
        {

            btn_pos_sel8.Text = pos_sel8.ToString();

            if (pos_sel8 == ((word_number - 1) / 2))
            {

                pos_sel8 = 1;

            }
            else
            {

                pos_sel8++;

            }

            pos_sel[7] = pos_sel8;

        }

        private void btn_pass_Click(object sender, EventArgs e)
        {

                first_word_btn1.Enabled = false;
                first_word_btn2.Enabled = false;
                second_word_btn1.Enabled = false;
                second_word_btn2.Enabled = false;
                third_word_btn1.Enabled = false;
                third_word_btn2.Enabled = false;
                fourth_word_btn1.Enabled = false;
                fourth_word_btn2.Enabled = false;
                fifth_word_btn1.Enabled = false;
                fifth_word_btn2.Enabled = false;
                sixth_word_btn1.Enabled = false;
                sixth_word_btn2.Enabled = false;
                seventh_word_btn1.Enabled = false;
                seventh_word_btn2.Enabled = false;
                eighth_word_btn1.Enabled = false;
                eighth_word_btn2.Enabled = false;

                btn_solution.Enabled = false;
                btn_pass.Enabled = false;

                first_word_btn1.BackColor = Color.LightGray;
                first_word_btn2.BackColor = Color.LightGray;
                second_word_btn1.BackColor = Color.LightGray;
                second_word_btn2.BackColor = Color.LightGray;
                third_word_btn1.BackColor = Color.LightGray;
                third_word_btn2.BackColor = Color.LightGray;
                fourth_word_btn1.BackColor = Color.LightGray;
                fourth_word_btn2.BackColor = Color.LightGray;
                fifth_word_btn1.BackColor = Color.LightGray;
                fifth_word_btn2.BackColor = Color.LightGray;
                sixth_word_btn1.BackColor = Color.LightGray;
                sixth_word_btn2.BackColor = Color.LightGray;
                seventh_word_btn1.BackColor = Color.LightGray;
                seventh_word_btn2.BackColor = Color.LightGray;
                eighth_word_btn1.BackColor = Color.LightGray;
                eighth_word_btn2.BackColor = Color.LightGray;

                btn_pos_sel1.Text = "";
                btn_pos_sel2.Text = "";
                btn_pos_sel3.Text = "";
                btn_pos_sel4.Text = "";
                btn_pos_sel5.Text = "";
                btn_pos_sel6.Text = "";
                btn_pos_sel7.Text = "";
                btn_pos_sel8.Text = "";

                btn_pos_sel1.Enabled = false;
                btn_pos_sel2.Enabled = false;
                btn_pos_sel3.Enabled = false;
                btn_pos_sel4.Enabled = false;
                btn_pos_sel5.Enabled = false;
                btn_pos_sel6.Enabled = false;
                btn_pos_sel7.Enabled = false;
                btn_pos_sel8.Enabled = false;

                snippets[0].word_pos = "0";
                snippets[0].path = "Lorem ipsum";
                snippets[1].word_pos = "0";
                snippets[1].path = "Lorem ipsum";
                snippets[2].word_pos = "0";
                snippets[2].path = "Lorem ipsum";
                snippets[3].word_pos = "0";
                snippets[3].path = "Lorem ipsum";
                snippets[4].word_pos = "0";
                snippets[4].path = "Lorem ipsum";
                snippets[5].word_pos = "0";
                snippets[5].path = "Lorem ipsum";
                snippets[6].word_pos = "0";
                snippets[6].path = "Lorem ipsum";
                snippets[7].word_pos = "0";
                snippets[7].path = "Lorem ipsum";

                txt_solution.Text = solutions[audio_number - 1];

                media_player.Ctlcontrols.stop();
                media_player.URL = paths[(word_number - 1)];                         //we play back the solution file
                media_player.Ctlcontrols.play();

                first_word_correct = false;
                second_word_correct = false;
                third_word_correct = false;
                fourth_word_correct = false;
                fifth_word_correct = false;
                sixth_word_correct = false;
                seventh_word_correct = false;
                eigtht_word_correct = false;

                first_select_correct = false;
                second_select_correct = false;
                third_select_correct = false;
                fourth_select_correct = false;
                fifth_select_correct = false;
                sixth_select_correct = false;
                seventh_select_correct = false;
                eigtht_select_correct = false;

                aid_cnt = 0;

                correct_answer_cnt--;

                txt_score.Text = correct_answer_cnt.ToString() + "/" + number_of_tests.ToString();
                btn_open.Text = "Next";
                lbl_pass_cnt.Text = "Passes: " + (step_number - correct_answer_cnt).ToString();

            }
    }
}

