import React, { Component, Fragment } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import getResource from '../../../resourceHelper';
import { noDataIndication, resolveClientUrl } from '../../../common-functions';
import { PaginationOptions, PAStatus, PerformanceProcess } from '../../../constants';
import EditMemberPA from './EditPAForm';

export default class PerformanceAppraisalDetail extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowEditModal: false,
            paRecordId: null
        };
    }

    handleClick = (paRecordId) => {
        this.setState({
            isShowEditModal: true,
            paRecordId: paRecordId
        });
    }

    onCloseModal = () => {
        this.setState({
            isShowEditModal: false,
            paRecord: {}
        });
    }

    columns = [
        {
            dataField: 'employee.nickName',
            text: `${getResource('NickName')}`
        },
        {
            dataField: 'self',
            text: 'Self',
            isDummyField: true,
            formatter: (cell, row) => {
                let evaluatorProcess = row.processes.find(p => p.processId === PerformanceProcess.Self);
                let grade = '';
                if (evaluatorProcess && evaluatorProcess.performanceRecord) {
                    grade = evaluatorProcess.performanceRecord.grade.name;
                }
                return (
                    <Fragment>{grade}</Fragment>
                );
            }
        }, {
            dataField: 'PM',
            text: 'Project Manager',
            isDummyField: true,
            formatter: (cell, row) => {
                let evaluatorProcess = row.processes.filter(p => p.processId === PerformanceProcess.ProjectManager);
                let grade = '';
                if (evaluatorProcess && evaluatorProcess.performanceRecord) {
                    grade = evaluatorProcess.map(p => p.performanceRecord.grade.name).join(',');
                }
                return (
                    <Fragment>{grade}</Fragment>
                );
            }
        }, {
            dataField: 'Evaluator',
            text: 'Evaluator',
            isDummyField: true,
            formatter: (cell, row) => {
                let evaluatorProcess = row.processes.find(p => p.processId === PerformanceProcess.Evaluator);
                let grade = '';
                if (evaluatorProcess && evaluatorProcess.performanceRecord) {
                    grade = evaluatorProcess.performanceRecord.grade.name;
                }
                return (
                    <Fragment>{grade}</Fragment>
                );
            }
        }, {
            dataField: 'Reviewer',
            text: 'Reviewer',
            isDummyField: true,
            formatter: (cell, row) => {
                let evaluatorProcess = row.processes.find(p => p.processId === PerformanceProcess.Reviewer);
                let grade = '';
                if (evaluatorProcess && evaluatorProcess.performanceRecord) {
                    grade = evaluatorProcess.performanceRecord.grade.name;
                }
                return (
                    <Fragment>{grade}</Fragment>
                );
            }
        }, {
            dataField: 'statusId',
            text: `PA Grade`,
            classes: 'text-center',
            headerStyle: { width: '80px' },
            formatter: (cell, row) => {
                switch (row.statusId) {
                    case PAStatus.InProgress:
                        return (
                            <Fragment>
                                <span>{row.grade ? row.grade.name : ''}</span>
                                <a className="fa fa-pencil-square-o" title="Edit" onClick={() => this.handleClick(row.paRecordId)} />
                            </Fragment>
                        );
                    case PAStatus.Closed:
                        return <span>{row.grade ? row.grade.name : ''}</span>;
                    default:
                        return <Fragment />;
                }
            }
        }
    ]

    render() {
        return (
            <Fragment>
                <ToolkitProvider keyField="paRecordId" columns={this.columns} data={this.props.data} search={{ searchFormatted: true }} >
                    {
                        props => {
                            return (
                                <Fragment>
                                    <BootstrapTable {...props.baseProps} noDataIndication={noDataIndication}
                                        striped
                                    />
                                </Fragment>
                            );
                        }
                    }
                </ToolkitProvider>
                {
                    this.state.isShowEditModal &&
                    <EditMemberPA
                        isShowModal={this.state.isShowEditModal}
                        paRecordId={this.state.paRecordId}
                        gradeOptions={this.props.gradeOptions}
                        onCloseModal={this.onCloseModal}
                    />
                }
            </Fragment>
        );
    }
}