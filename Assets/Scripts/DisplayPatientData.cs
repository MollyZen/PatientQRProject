using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Patient {
    public string name;
    public string lastname;
    public string patronymic;
    public string date_of_birth;
    public string date_of_receipt;
    public string diagnosis;
    public string appointment;
    public string comment;
    public string uuid;
}

/// <summary>
/// Used to display patient data to data sheet
/// </summary>
public class DisplayPatientData : MonoBehaviour
{
    /// <summary>
    /// Used to display patient data to data sheet
    /// </summary>
    static public void SetPatientData(Patient patient, TextMeshPro dataSheet)
    {
        dataSheet.SetText("<color=red>Имя: " + "<color=blue>" +  patient.name + "\n" +
                          "<color=red>Фамилия: " + "<color=blue>" + patient.lastname + "\n" +
                          "<color=red>Отчество: " + "<color=blue>" + patient.patronymic + "\n" +
                          "<color=red>Дата рождения: " + "<color=blue>" + patient.date_of_birth + "\n" +
                          "<color=red>Дата и время поступления: " + "<color=blue>" + 
                                      System.DateTime.ParseExact(patient.date_of_receipt, "yyyy-MM-ddTHH:mm:sszzz",
                                      System.Globalization.CultureInfo.InvariantCulture)
                                      + "\n" +
                          "<color=red>Диагноз: " + "<color=blue>" + patient.diagnosis + "\n" +
                          "<color=red>Назначение: " + "<color=blue>" + patient.appointment + "\n" +
                          "<color=red>Комментарии: " + "<color=blue>" + patient.comment + "\n" +
                          "<color=red>UUID: " + "<color=blue>" + patient.uuid);
    }

    public void Start()
    {
        enabled = false;
    }
}
