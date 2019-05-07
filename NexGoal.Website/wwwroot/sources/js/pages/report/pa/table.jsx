import React, { Component, Fragment } from 'react';
import getResource from '../../../resourceHelper';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import { PerformanceProcess } from '../../../constants';
import { noDataIndication, getPAGradeClassName, resolveClientUrl } from '../../../common-functions';
import StickyBlock from '../../../shared/components/StickyBlock';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.totalColums = 11;
    }

    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(nextProps.periodRecords) !== JSON.stringify(this.props.periodRecords)) {
            return true;
        }
        return false;
    }

    parseIntGradeId = gradeId => gradeId !== null ? gradeId : 0

    renderToeic = (toeicPolicy, toeic) => {
        let toeicClass = '';
        if (toeicPolicy !== null && toeicPolicy > 0) toeicClass = 'toeic-passed';
        if (toeicPolicy === null || toeicPolicy < 0) toeicClass = 'toeic-failed';
        return <span className={`bold ${toeicClass}`}>{toeic ? toeic : getResource('N/A')}</span>;
    }

    renderGroupRecords = groups => {
        if (groups) {
            return groups.map((group, index) => this.renderGroupPA(group, index));
        } else {
            return (
                <tr>
                    <td className="empty-result" colSpan={this.totalColums}>{getResource('NoRecordFound')}</td>
                </tr>
            );
        }
    }

    renderRows = periodRecords => {
        if (!periodRecords.length) {
            return (
                <tr>
                    <td className="empty-result" colSpan={this.totalColums}>{getResource('NoRecordFound')}</td>
                </tr>
            );
        } else {
            return (
                periodRecords.map((periodRecord, index) => {
                    let title = periodRecord.period.name;
                    if (periodRecord.period.isInProgress) {
                        title += ' - In Progress';
                    }

                    return (
                        <Fragment key={index}>
                            <tr className="group-row">
                                <td colSpan={this.totalColums}>{title}</td>
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

    renderPARivisedIcon = (originalGradeId, gradeId) => {
        const gradeChangeClass = getPAGradeClassName(originalGradeId, gradeId);

        return <span className={gradeChangeClass} />;
    }

    renderGroupPA = (groupPA, index) => {
        return (
            <Fragment key={index}>
                <tr className="team-group-row">
                    <td colSpan={this.totalColums}>{groupPA.group.name}</td>
                </tr>
                {
                    this.renderPARecordInfo(groupPA.paRecords)
                }
            </Fragment>
        );
    }

    renderPAGrade = record => {
        const paGrade = record.grade ? record.grade.name : getResource('N/A');
        let gradeId = record.grade ? record.grade.gradeId : null;
        let originalGradeId = record.originalGrade ? record.originalGrade.gradeId : null;

        const revisedIcon = this.renderPARivisedIcon(originalGradeId, gradeId);
        return <span>{paGrade}{revisedIcon}</span>;
    }

    renderPARecordInfo = paRecords => {
        return (
            paRecords.map((record, index) => {
                return (
                    <tr key={index}>
                        <td className="text-center">{index + 1}</td>
                        <td>{record.employee.nickName}</td>
                        <td>{record.employee.jobTitle}</td>
                        <td className="text-center">{this.renderToeic(record.toeicPolicy, record.toeic)}</td>
                        <td className="text-center">
                            {this.renderGradeByProcess(record.processes, PerformanceProcess.Self, record.paRecordId)}
                        </td>
                        <td className="text-center">
                            {this.renderGradeByProcess(record.processes, PerformanceProcess.ProjectManager)}
                        </td>
                        <td className="text-center">{this.renderGradeByProcess(record.processes, PerformanceProcess.Evaluator)}</td>
                        <td className="text-center">{this.renderGradeByProcess(record.processes, PerformanceProcess.Reviewer)}</td>
                        <td className="text-center">{this.renderPAGrade(record)}</td>
                        <td className="text-center">{record.isRevised ? <span className="icon-check" /> : ''}</td>
                        <td className="text-center">
                            <span className="icon-search"
                                onClick={() => this.showPADetails(record.paRecordId)}
                            />
                            <a href={resolveClientUrl(`PAReport/ExportPdf/${record.paRecordId}`)} >
                                <span className="icon-pdf" />
                            </a>
                        </td>
                    </tr >
                );
            })
        );
    }

    showPADetails = paRecordId => {
        this.props.showPaDetail(paRecordId);
    }

    countDuplicateGrades = grades => {
        grades.sort();
        let displayGrades = [];
        let currentGrade = null;
        var count = 0;
        for (let i = 0; i < grades.length; i++) {
            if (grades[i]) {
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
        }

        if (count > 0) {
            displayGrades.push(count > 1 ? count + currentGrade : currentGrade);
        }

        return displayGrades.join(', ');
    }

    renderPMGrades = (paProcesses, paRecordId) => {
        const processes = paProcesses.filter(process => process.performanceRecord &&
            process.processId === PerformanceProcess.ProjectManager);
        const grades = processes.map((process, index) => {
            return process.performanceRecord.grade ?
                process.performanceRecord.grade.name : getResource('N/A');
        });
        const tooltip = (
            <Tooltip className="focus-note" id={`pmGrade-${paRecordId}`}>
                {
                    processes.map((process, index) => {
                        return (
                            <p key={index}>
                                {
                                    `${process.evaluator.nickName}: ${process.performanceRecord.grade
                                        ? process.performanceRecord.grade.name : getResource('N/A')} `
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

    renderGradeByProcess = (processes, processId, paRecordId) => {
        const processRecords = processes.filter(process => process.performanceRecord && process.processId === processId);
        if (!processRecords.length) return '';

        if (processId !== PerformanceProcess.ProjectManager) {
            const grade = this.renderGrade(processRecords[0]);
            return grade;
        } else {
            return this.renderPMGrades(processRecords, paRecordId);
        }
    }

    renderGrade = process => {
        if (process.performanceRecord) {
            return process.performanceRecord.grade ? process.performanceRecord.grade.name : getResource('N/A');
        }

        return '';
    }

    render() {
        let content = '';
        if (this.props.periodRecords) {
            content = this.props.periodRecords.length > 0 ? this.renderRows(this.props.periodRecords)
                : <tr><td className="text-center" colSpan={this.totalColums}>{noDataIndication()}</td></tr>;
        }

        return (
            <Fragment>
                <StickyBlock threshold={236}>
                    <table className="table table-bordered pa-report-table">
                        <thead>
                            <tr>
                                <th className="order" rowSpan="2">{getResource('SymbolCount')}</th>
                                <th className="nick-name" rowSpan="2">{getResource('NickName')}</th>
                                <th className="job-title" rowSpan="2">{getResource('JobTitle')}</th>
                                <th className="toeic" rowSpan="2">{getResource('Toeic')}</th>
                                <th colSpan="4">{getResource('PerformanceRecord')}</th>
                                <th width="10%" rowSpan="2">
                                    {getResource('PAFinal')}
                                    <div className="pa-final">
                                        <i className="icon-info"></i>
                                        <div className="description">
                                            <i className="arrow"></i>
                                            <p>PA Final = Evaluator’s grade + Toeic Policy</p>
                                            <p>If the Evaluator's grade is N/A or Null (empty):</p>
                                            <ul>
                                                <li>In Progress PA period: PA Final = N/A </li>
                                                <li>Closed PA period: PA Final = U (Unsatisfactory)</li>
                                            </ul>
                                        </div>
                                    </div>
                                </th>
                                <th className="revised" rowSpan="2">{getResource('Revised')}</th>
                                <th className="action" rowSpan="2">{getResource('Action')}</th>
                            </tr>
                            <tr>
                                <th className="self">{getResource('Self')}</th>
                                <th className="pm">{getResource('ProjectManager')}</th>
                                <th className="evaluator">{getResource('Evaluator')}</th>
                                <th className="reviewer">{getResource('Reviewer')}</th>
                            </tr>
                        </thead>
                    </table>
                </StickyBlock>
                <table className="table table-bordered pa-report-table">
                    <thead>
                        <tr>
                            <th className="order" rowSpan="2">{getResource('SymbolCount')}</th>
                            <th className="nick-name" rowSpan="2">{getResource('NickName')}</th>
                            <th className="job-title" rowSpan="2">{getResource('JobTitle')}</th>
                            <th className="toeic" rowSpan="2">{getResource('Toeic')}</th>
                            <th colSpan="4">{getResource('PerformanceRecord')}</th>
                            <th width="10%" rowSpan="2">
                                {getResource('PAFinal')}
                                <div className="pa-final">
                                    <i className="icon-info"></i>
                                    <div className="description">
                                        <i className="arrow"></i>
                                        <p>PA Final = Evaluator’s grade + Toeic Policy</p>
                                        <p>If the Evaluator's grade is N/A or Null (empty):</p>
                                        <ul>
                                            <li>In Progress PA period: PA Final = N/A </li>
                                            <li>Closed PA period: PA Final = U (Unsatisfactory)</li>
                                        </ul>
                                    </div>
                                </div>
                            </th>
                            <th className="revised" rowSpan="2">{getResource('Revised')}</th>
                            <th className="action" rowSpan="2">{getResource('Action')}</th>
                        </tr>
                        <tr>
                            <th className="self">{getResource('Self')}</th>
                            <th className="pm">{getResource('ProjectManager')}</th>
                            <th className="evaluator">{getResource('Evaluator')}</th>
                            <th className="reviewer">{getResource('Reviewer')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {content}
                    </tbody>
                </table>
            </Fragment>
        );
    }
}