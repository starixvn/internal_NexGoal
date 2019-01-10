import React, { Component, Fragment } from 'react';
import getResource from '../../../resourceHelper';
import { PerformanceProcess } from '../../../constants';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(nextProps.periodRecords) !== JSON.stringify(this.props.periodRecords)) {
            return true;
        }
        return false;
    }

    renderRows = (periodRecords) => {
        if (!periodRecords.length) {
            return (
                <tr>
                    <td className="empty-result" colSpan={10}>{getResource('NoRecordFound')}</td>
                </tr>
            );
        } else {
            return (
                periodRecords.map(periodRecord => {
                    return (
                        <Fragment>
                            <tr className="group-row">
                                <td colSpan={10}>{periodRecord.period.name}</td>
                            </tr>
                            {
                                this.renderPARecordInfo(periodRecord.paRecords)
                            }
                        </Fragment>
                    );
                })
            );
        }
    }

    showPADetails = (paRecordId) => {
        console.log(paRecordId);
    }

    renderPARecordInfo = (paRecords) => {
        return (
            paRecords.map(record => {
                return (
                    <tr>
                        <td>{record.employee.fullName}</td>
                        <td className="text-center">{record.employee.nickName}</td>
                        <td>{record.employee.jobTitle}</td>
                        <td>{record.employee.groupName}</td>
                        <td className="text-center">{this.renderGradeByProcess(record.processes, PerformanceProcess.Self)}</td>
                        <td>{this.renderGradeByProcess(record.processes, PerformanceProcess.ProjectManager)}</td>
                        <td className="text-center">{this.renderGradeByProcess(record.processes, PerformanceProcess.Evaluator)}</td>
                        <td className="text-center">{this.renderGradeByProcess(record.processes, PerformanceProcess.Reviewer)}</td>
                        <td className="text-center">{record.grade ? record.grade.name : getResource('N/A')}</td>
                        <td className="text-center"><span className="icon-search" onClick={() => this.showPADetails(record.paRecordId)} /></td>
                    </tr>
                );
            })
        );
    }

    renderGradeByProcess = (processes, processId) => {
        const processRecord = processes.filter(process => process.processId === processId);
        if (!processRecord.length) return '';

        if (processId !== PerformanceProcess.ProjectManager) {
            const grade = this.renderGrade(processRecord[0]);
            return grade;
        } else {
            return (
                processRecord.map(process => {
                    const grade = this.renderGrade(process);
                    return <p className="margin-0">{`${process.evaluator.nickName} : ${grade}`}</p>;
                })
            );
        }
    }

    renderGrade = (process) => {
        if (process.performanceRecord) {
            return process.performanceRecord.grade ? process.performanceRecord.grade.name : getResource('N/A');
        }

        return '';
    }

    render() {
        const periodRecords = this.props.periodRecords;
        return (
            <table className="table table-bordered">
                <thead>
                    <tr>
                        <th>{getResource('FullName')}</th>
                        <th>{getResource('NickName')}</th>
                        <th>{getResource('JobTitle')}</th>
                        <th>{getResource('Team')}</th>
                        <th>{getResource('Self')}</th>
                        <th>{getResource('ProjectManager')}</th>
                        <th>{getResource('Evaluator')}</th>
                        <th>{getResource('Reviewer')}</th>
                        <th>{getResource('PAFinalGrade')}</th>
                        <th>{getResource('Action')}</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        this.renderRows(periodRecords)
                    }
                </tbody>
            </table>
        );
    }
}