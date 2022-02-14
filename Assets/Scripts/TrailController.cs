using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class TrailController : MonoBehaviour
{
    //Load midi file and change position depending on notes (pitch to numbers).
    //Takes whole midi file as one track -> When there are multiple track it is nessesary to seperate them to separate midi files (one instrument one midi file).
    //(I was to lazy to find more elegant solution using DryWetMidi Track system)

    //path to midi file
    public string Path;

    //Visualisation variables
    public float MoveByNoteDistance = 0.1f;

    public float QuiverTime = 0.1f;
    public float QuiverSize= 0.05f;


    //notes variables, I used two separate array for convinience
    private int[] _noteNumbers;
    private MetricTimeSpan[] _noteMetricTimes;


    //playing variables
    private float _timeCounter;
    private float _timeCheck;
    private int _noteCounter;

    // Start is called before the first frame update
    void Start()
    {
        //load midi file and parse it to arrays
        var midiFile = MidiFile.Read(Path);

        List<Note> notes = new List<Note>(midiFile.GetNotes());
        TempoMap tempoMap = midiFile.GetTempoMap();

        midiFile = null;

        _noteNumbers = new int[notes.Count];
        _noteMetricTimes = new MetricTimeSpan[notes.Count];

        for(int i = 0; i < notes.Count; i++)
        {
            //Change noteNumbers to 0-12 and save values;
            _noteNumbers[i] = notes[i].NoteNumber % 12;

            //Save starting times of notes
            _noteMetricTimes[i] = notes[i].TimeAs<MetricTimeSpan>(tempoMap);

        }

        notes.Clear();

        //Init playing variables
        _noteCounter = 0;
        _timeCheck = _noteMetricTimes[_noteCounter].TotalMicroseconds * 0.000001f;
    }

    // Update is called once per frame
    void Update()
    {
        //Midi events are done via timer and updating time checkpoints.
        _timeCounter += Time.deltaTime;

        if (_timeCounter >= _timeCheck)
        {   
            if(_noteCounter != 0 && _noteNumbers[_noteCounter - 1] == _noteNumbers[_noteCounter])
            {
                //Quiver when current note is same as one before.
                StartCoroutine(QuiverTrail());
            }
            else
            {
                //Change position of trail depending on note number.
                MoveTrailByNoteNumber(_noteNumbers[_noteCounter]);
            }

            //Create next checkpoint or wait 30s if end.
            if(_noteCounter + 1 < _noteNumbers.Length)
            {
                _noteCounter++;
                _timeCheck = _noteMetricTimes[_noteCounter].TotalMicroseconds * 0.000001f;
            }
            else
            {
                MoveTrailByNoteNumber(8);
                _timeCheck = 30f;
            }    
        }
    }

    //change z position of trail depending on note number
    private void MoveTrailByNoteNumber(int noteNumber)
    {
        Vector3 temp = this.transform.position;
        temp.z = (noteNumber + 1) * MoveByNoteDistance;
        this.transform.position = temp;
    }

    //move trail on z axis up down up
    IEnumerator QuiverTrail()
    {
        this.transform.position += new Vector3(0f, 0f, QuiverSize);

        yield return new WaitForSeconds(QuiverTime);

        this.transform.position += new Vector3(0f, 0f, - (2 * QuiverSize));

        yield return new WaitForSeconds(QuiverTime);

        this.transform.position += new Vector3(0f, 0f, QuiverSize);

    }
}
