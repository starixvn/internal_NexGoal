import React, { Component, Fragment } from 'react';
import getResource from '../../../resourceHelper';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import { PerformanceProcess } from '../../../constants';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.totalColums = 10;
    }

    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(nextProps.periodRecords) !== JSON.stringify(this.props.periodRecords)) {
            return true;
        }
        return false;
    }

    parseIntGradeId = (gradeId) => {
        return gradeId !== null ? gradeId : 0;
    }

    renderToeic = (toeicPolicy, toeic) => {
        let toeicClass = '';
        if (toeicPolicy !== null && toeicPolicy > 0) toeicClass = 'toeic-passed';
        if (toeicPolicy === null || toeicPolicy < 0) toeicClass = 'toeic-failed';
        return <span className={`bold ${toeicClass}`}>{toeic ? toeic : getResource('N/A')}</span>;
    }

    renderGroupRecords = (groups) => {
        if (groups) {
            return groups.map(group => {
                return this.renderGroupPA(group);
            });
        } else {
            return (
                <tr>
                    <td className="empty-result" colSpan={this.totalColums}>{getResource('NoRecordFound')}</td>
                </tr>
            );
        }
    }

    renderRows = (periodRecords) => {
        if (!periodRecords.length) {
            return (
                <tr>
                    <td className="empty-result" colSpan={this.totalColums}>{getResource('NoRecordFound')}</td>
                </tr>
            );
        } else {
            return (
                periodRecords.map(periodRecord => {
                    return (
                        <Fragment>
                            <tr className="group-row">
                                <td colSpan={this.totalColums}>{periodRecord.period.name}</td>
                            </tr>
                            {
                                this.renderGroupRecords(periodRecord.groups)
                            }
                        </Fragment>
                    );
                })
            );
        }
    }

    renderPARivisedIcon = (gradeId, originalGradeId) => {
        let gradeChangeClass = '';
        if (this.parseIntGradeId(gradeId) > this.parseIntGradeId(originalGradeId)) {
            gradeChangeClass = 'icon-increase-arrow';
        }

        if (this.parseIntGradeId(gradeId) < this.parseIntGradeId(originalGradeId)) {
            gradeChangeClass = 'icon-descrease-arrow';
        }

        return <span className={gradeChangeClass} />;
    }

    renderGroupPA = (groupPA) => {
        return (
            <Fragment>
                <tr className="team-group-row">
                    <td colSpan={this.totalColums}>{groupPA.group.name}</td>
                </tr>
                {
                    this.renderPARecordInfo(groupPA.paRecords)
                }
            </Fragment>
        );
    }

    renderPAGrade = (record) => {
        const paGrade = record.grade ? record.grade.name : getResource('N/A');
        const revisedIcon = this.renderPARivisedIcon(record.gradeId, record.originalGradeId);
        return (
            <Fragment>
                <span>{paGrade}{revisedIcon}</span>
            </Fragment>
        );
    }
    renderPARecordInfo = (paRecords) => {
        return (
            paRecords.map((record, index) => {
                return (
                    <tr>
                        <td className="text-center">{index + 1}</td>
                        <td>{record.employee.nickName}</td>
                        <td>{record.employee.jobTitle}</td>
                        <td className="text-center">{this.renderToeic(record.toeicPolicy, record.toeic)}</td>
                        <td className="text-center">{this.renderGradeByProcess(record.processes, PerformanceProcess.Self)}</td>
                        <td className="text-center">
                            {this.renderGradeByProcess(record.processes, PerformanceProcess.ProjectManager)}
                        </td>
                        <td className="text-center">{this.renderGradeByProcess(record.processes, PerformanceProcess.Evaluator)}</td>
                        <td className="text-center">{this.renderGradeByProcess(record.processes, PerformanceProcess.Reviewer)}</td>
                        <td className="text-center">{this.renderPAGrade(record)}</td>
                        <td className="text-center">
                            <span className="icon-search"
                                onClick={() => this.showPADetails(record.paRecordId)}
                            />
                            <a href={`PaDetail?paRecordId=${record.paRecordId}`} >
                                <span className="icon-pdf" />
                            </a>
                        </td>
                    </tr >
                );
            })
        );
    }

    showPADetails = (paRecordId) => {
        this.props.showPaDetail(paRecordId);
    }

    countDuplicateGrades = (grades) => {
        grades.sort();
        let displayGrades = [];
        let currentGrade = null;
        var count = 0;
        for (let i = 0; i < grades.length; i++) {
            if (grades[i] !== currentGrade) {
                if (count > 0) {
                    displayGrades.push(count > 1 ? count + currentGrade : currentGrade);
                }
                currentGrade = grades[i];
                count = 1;
            } else {
                count++;
            }
        }
        if (count > 0) {
            displayGrades.push(count > 1 ? count + currentGrade : currentGrade);
        }

        return displayGrades.join(', ');
    }

    renderPMGrades = (paProcesses) => {
        const processes = paProcesses.filter(process => process.processId === PerformanceProcess.ProjectManager);
        const grades = processes.map(process => {
            return process.performanceRecord && process.performanceRecord.grade ?
                process.performanceRecord.grade.name : getResource('N/A');
        });
        const tooltip = (
            <Tooltip className="focus-note">
                {
                    processes.map(process => {
                        return (
                            <p>
                                {
                                    `${process.evaluator.nickName}: ${process.performanceRecord && process.performanceRecord.grade
                                        ? process.performanceRecord.grade.name : getResource("N/A")} `
                                }
                            </p>
                        );
                    })
                }
            </Tooltip>
        );

        return (
            <OverlayTrigger placement="top" overlay={tooltip}>
                <span className="pointer">{this.countDuplicateGrades(grades)}</span>
            </OverlayTrigger>
        );
    }

    renderGradeByProcess = (processes, processId) => {
        const processRecords = processes.filter(process => process.processId === processId);
        if (!processRecords.length) return '';

        if (processId !== PerformanceProcess.ProjectManager) {
            const grade = this.renderGrade(processRecords[0]);
            return grade;
        } else {
            return this.renderPMGrades(processRecords);
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
                        <th rowSpan="2">#</th>
                        <th rowSpan="2">{getResource('NickName')}</th>
                        <th rowSpan="2">{getResource('JobTitle')}</th>
                        <th rowSpan="2">{getResource('Toeic')}</th>
                        <th rowSpan="1" colSpan="4">{getResource('PerformanceRecord')}</th>
                        <th rowSpan="2">{getResource('PAFinalGrade')}</th>
                        <th rowSpan="2">{getResource('Action')}</th>
                    </tr>
                    <tr>
                        <th rowSpan="1">{getResource('Self')}</th>
                        <th rowSpan="1">{getResource('ProjectManager')}</th>
                        <th rowSpan="1">{getResource('Evaluator')}</th>
                        <th rowSpan="2">{getResource('Reviewer')}</th>
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