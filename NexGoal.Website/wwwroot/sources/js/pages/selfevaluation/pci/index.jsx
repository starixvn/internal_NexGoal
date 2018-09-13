import React, { Component, Fragment } from 'react';
import { Pagination, DropdownButton, MenuItem } from 'react-bootstrap';
import { range } from 'lodash';
import moment from 'moment';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';
import { PerformanceRecordSizePerPages } from '../../../constants';
import SearchForm from './SearchForm';

export default class PerformanceCheckIn extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                fromDate: moment().subtract(5, 'months').startOf('month'),
                toDate: moment(),
                creators: [],
                projects: [],
                pageSize: PerformanceRecordSizePerPages[0].value,
                pageIndex: 1
            },
            totalItems: 0,
            performanceRecords: [],
            creatorOptions: [],
            projectOptions: []
        };

        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.handleFilterSubmit = this.handleFilterSubmit.bind(this);
        this.handlePageSizeChange = this.handlePageSizeChange.bind(this);
        this.handlePageIndexChange = this.handlePageIndexChange.bind(this);
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Self/PerformanceCheckIn/GetCreators')),
            axios(resolveClientUrl('Common/GetProjects'))
        ];

        Promise.all(promises)
            .then(([creatorResponse, projectResponse]) => {
                const creatorOptions = creatorResponse.data.map((creator) => {
                    return { value: creator.employeeId, label: `${creator.fullName} (${creator.nickName})` };
                });
                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });
                const nextState = Object.assign({}, this.state.filter, { creators: creatorOptions });

                this.setState({
                    creatorOptions: creatorOptions,
                    projectOptions: projectOptions,
                    filter: nextState
                });

                this.getPerformanceRecords(nextState);
            }).catch(axiosError);
    }

    getPerformanceRecords(filter) {
        axios(resolveClientUrl('Self/PerformanceCheckIn/Index'), {
            params: {
                fromDate: filter.fromDate.format('YYYY-MM-DD'),
                toDate: filter.toDate.format('YYYY-MM-DD'),
                creatorIds: filter.creators.map((creator) => { return creator.value; }).join(','),
                projectIds: filter.projects.map((project) => { return project.value; }).join(','),
                pageSize: filter.pageSize,
                pageIndex: filter.pageIndex - 1
            }
        }).then(response => {
            this.setState({
                performanceRecords: response.data.performanceRecords,
                totalItems: response.data.totalItems
            });
        }).catch(axiosError);
    }

    handleFilterChange(name, value) {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleFilterSubmit() {
        const filter = this.state.filter;

        this.getPerformanceRecords(filter);
    }

    handlePageSizeChange(event) {
        event.preventDefault();

        const nextPageSize = Number.parseInt(event.target.dataset.page);

        if (nextPageSize !== this.state.filter.pageSize) {
            const nextState = Object.assign({}, this.state.filter, {
                pageSize: nextPageSize,
                pageIndex: 1
            });

            this.setState({ filter: nextState });

            this.getPerformanceRecords(nextState);
        }
    }

    handlePageIndexChange(pageIndex) {
        const nextState = Object.assign({}, this.state.filter, { pageIndex: pageIndex });

        this.setState({ filter: nextState });

        this.getPerformanceRecords(nextState);
    }

    render() {
        const filter = this.state.filter;
        const performanceRecords = this.state.performanceRecords;
        const totalPage = Math.ceil(this.state.totalItems / filter.pageSize);
        const pageIndex = filter.pageIndex;

        return (
            <Fragment>
                <SearchForm filter={filter}
                    creatorOptions={this.state.creatorOptions}
                    projectOptions={this.state.projectOptions}
                    onFilterChange={this.handleFilterChange}
                    onFilterSubmit={this.handleFilterSubmit}
                />
                <a href={resolveClientUrl('Self/PerformanceCheckIn/Create')} className="btn btn-primary btn-add">
                    <span className="icon-playlist-plus" />Performance Check-in
                </a>
                {
                    performanceRecords.length > 0 &&
                    <Fragment>
                        <div className="performance-record">
                            {
                                performanceRecords.map((performanceRecord) => {
                                    return (
                                        <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                                            grade={performanceRecord.grade ? performanceRecord.grade.name : ''}
                                            comment={performanceRecord.comment}
                                            projects={performanceRecord.projects.map(project => { return project.name; }).join(',')}
                                        />
                                    );
                                })
                            }
                        </div>
                        <div className="clearfix">
                            <div className="pull-left page-size">
                                <DropdownButton
                                    bsStyle="default"
                                    title={filter.pageSize}
                                    id="page-size-select"
                                >
                                    {
                                        PerformanceRecordSizePerPages.map((pageSize) => {
                                            return (
                                                <MenuItem key={`pageSize${pageSize.value}`} onClick={this.handlePageSizeChange} data-page={pageSize.value}>
                                                    {pageSize.text}
                                                </MenuItem>
                                            );
                                        })
                                    }
                                </DropdownButton>
                            </div>
                            <div className="pull-right">
                                <Pagination>
                                    {pageIndex > 1 && <Pagination.First onClick={e => this.handlePageIndexChange(1)} />}
                                    {pageIndex > 1 && <Pagination.Prev onClick={e => this.handlePageIndexChange(this.state.page - 1)} />}
                                    {
                                        range(1, totalPage + 1).map(page =>
                                            <Pagination.Item key={page} active={pageIndex === page} onClick={e => this.handlePageIndexChange(page)}>
                                                {page}
                                            </Pagination.Item>
                                        )}
                                    {pageIndex < totalPage && <Pagination.Next onClick={e => this.handlePageIndexChange(pageIndex + 1)} />}
                                    {pageIndex < totalPage && <Pagination.Last onClick={e => this.handlePageIndexChange(totalPage)} />}
                                </Pagination>
                            </div>
                        </div>
                    </Fragment>
                }
            </Fragment>
        );
    }
}