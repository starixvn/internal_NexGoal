import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';
import Table from './Table';
import CommentModal from './CommentModal';

const ProbationResultOptions = [
    { label: 'All', value: '' },
    { label: 'Passed', value: true },
    { label: 'Failed', value: false }
];

export default class ProbationReport extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showModal: false,
            filter: {
                fromDate: moment().startOf('month'),
                toDate: moment().endOf('month'),
                groups: [],
                employees: [],
                isQualifiedByEvaluator: ProbationResultOptions[0],
                isQualified: ProbationResultOptions[0]
            },
            groupOptions: [],
            employeeOptions: [],
            tableData: null,
            commentModal: {
                show: false,
                content: ''
            }
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('ProbationReport/GetGroupsAndEmployees'))
        ];

        Promise.all(promises)
            .then(([response]) => {
                const groupOptions = response.data.groups.map(group => {
                    return {
                        value: group.groupId,
                        label: group.name
                    };
                });

                this.employees = response.data.employees;

                const employeeOptions = response.data.employees.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                const filter = {
                    ... this.state.filter,
                    ...{
                        groups: groupOptions,
                        employees: employeeOptions
                    }
                };

                this.setState({ filter, groupOptions, employeeOptions }, this.getProbationers);
            }).catch(axiosError);
    }

    getProbationers = () => {
        const filter = this.state.filter;

        axios.get(resolveClientUrl('ProbationReport/Get'), {
            params: {
                fromDate: filter.fromDate.format('YYYY-MM-DD'),
                toDate: filter.toDate.format('YYYY-MM-DD'),
                employeeIds: filter.employees.map(x => x.value).toString(),
                isQualifiedByEvaluator: filter.isQualifiedByEvaluator.value,
                isQualified: filter.isQualified.value
            }
        }).then(response => {
            response.data.forEach(probationer => {
                probationer.joinedDate = moment(probationer.joinedDate);
            });

            this.setState({ tableData: response.data });
        }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            let employeeOptions = this.employees.filter(e => value.some(v => v.value === e.groupId))
                .map(e => ({
                    value: e.employeeId,
                    label: `${e.nickName} (${e.fullName})`
                }));

            this.setState({
                employeeOptions: employeeOptions,
                filter: {
                    ...this.state.filter,
                    ...{
                        [name]: value,
                        employees: employeeOptions
                    }
                }
            });
        } else {
            this.setState({
                filter: { ...this.state.filter, ...{ [name]: value } }
            });
        }
    }

    handleShowCommentModal = comment => this.setState({ commentModal: { show: true, content: comment } })

    handleCloseCommentModal = () => this.setState({ commentModal: { show: false } })

    render() {
        const filter = this.state.filter;

        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('ProbationReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={this.state.filter}
                        groupOptions={this.state.groupOptions}
                        employeeOptions={this.state.employeeOptions}
                        onFilterChange={this.handleFilterChange}
                        probationResultOptions={ProbationResultOptions}
                        exportUrl={`/ProbationReport/ExportExcel?${stringify({
                            fromDate: filter.fromDate.format('YYYY-MM-DD'),
                            toDate: filter.toDate.format('YYYY-MM-DD'),
                            employeeIds: filter.employees.map(x => x.value).toString(),
                            isQualifiedByEvaluator: filter.isQualifiedByEvaluator.value,
                            isQualified: filter.isQualified.value
                        })}`}
                        onSubmit={this.getProbationers}
                    />

                    <Table tableData={this.state.tableData}
                        onShowComment={this.handleShowCommentModal}
                    />
                </div>
                <CommentModal showModal={this.state.commentModal.show} content={this.state.commentModal.content}
                    onCloseModal={this.handleCloseCommentModal}
                />
            </div>
        );
    }
}

ReactDOM.render(<ProbationReport />, document.getElementById('probation-report-wrapper'));