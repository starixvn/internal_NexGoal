import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import TextareaFullscreenEditor from '../../shared/components/textarea-fullscreen/index';

export default class CreateMemberPerformance extends Component {
    static propTypes = {
        currentUser: PropTypes.object,
        memberOptions: PropTypes.arrayOf(PropTypes.object),
        performanceGradeOptions: PropTypes.arrayOf(PropTypes.object),
        projectOptions: PropTypes.arrayOf(PropTypes.object),
        onSubmitSuccess: PropTypes.func
    };

    static defaultProps = {
        currentUser: { nickName: '', avatar: '' },
        memberOptions: [],
        performanceGradeOptions: [],
        projectOptions: [],
        onSubmitSuccess: function () { }
    };

    constructor(props) {
        super(props);

        this.state = {
            isOpenForm: false,
            performanceRecord: {
                employees: [],
                grade: null,
                comment: '',
                projects: [],
                statusId: null
            },
            currentDate: moment(),
            errors: {}
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.currentUser) !== JSON.stringify(nextProps.currentUser)
            || JSON.stringify(this.props.memberOptions) !== JSON.stringify(nextProps.memberOptions)
            || JSON.stringify(this.props.performanceGradeOptions) !== JSON.stringify(nextProps.performanceGradeOptions)
            || JSON.stringify(this.props.projectOptions) !== JSON.stringify(nextProps.projectOptions)
            || JSON.stringify(this.state.performanceRecord) !== JSON.stringify(nextState.performanceRecord)
            || JSON.stringify(this.state.errors) !== JSON.stringify(nextState.errors)
            || this.state.isOpenForm !== nextState.isOpenForm) {
            return true;
        }

        return false;
    }

    handleOpenForm = () => {
        this.setState({ isOpenForm: true });
        this.selectMember();
    }

    handleEmployeeChange = (selectedOptions) => {
        let errors = { ...this.state.errors };
        delete errors['employee'];

        if (selectedOptions.length === 0) {
            errors.employee = getResource('EmployeeRequired');
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { employees: selectedOptions }),
            errors: errors
        });
    }

    handleGradeChange = (selectedOption) => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    }

    selectMember = () => {
        if (this.props.selectedMembers !== undefined && this.state.performanceRecord.employees.length === 0) {
            this.setState({
                performanceRecord: Object.assign({}, this.state.performanceRecord, { employees: this.props.selectedMembers })
            });
        }
    }

    handleCommentChange = (event) => {
        let errors = { ...this.state.errors };
        delete errors['comment'];
        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { comment: event.target.value }),
            errors: errors
        });
    }

    handleProjectChange = (selectedOptions) => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { projects: selectedOptions })
        });
    }

    validateForm = () => {
        let errors = {}, isValid = true;
        let performanceRecord = this.state.performanceRecord;

        if (performanceRecord.employees.length === 0) {
            errors.employee = getResource('EmployeeRequired');
            isValid = false;
        }

        if (!performanceRecord.comment || performanceRecord.comment.trim() === '') {
            errors.comment = getResource('CommentRequired');
            isValid = false;
        }

        this.setState({ errors });

        return isValid;
    }

    handleSubmit = (event) => {
        event.preventDefault();

        if (this.validateForm()) {
            let statusId = 0;
            switch (event.target.name) {
                case 'save':
                    statusId = PerformanceRecordStatus.Submitted;
                    break;
                case 'saveAsDraft':
                    statusId = PerformanceRecordStatus.Draft;
                    break;
            }

            let record = this.state.performanceRecord;
            let performanceRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                employeeIds: record.employees.map((employee) => { return employee.value; }).join(','),
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment,
                projectIds: record.projects.map((project) => { return project.value; }).join(','),
                statusId: statusId
            });

            axios({
                url: resolveClientUrl('Member/Performance/Create'),
                method: 'POST',
                data: performanceRecord,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;
                if (data.isSuccessful) {
                    toast.success(getResource('CreatedSuccessfully'));
                    this.props.onSubmitSuccess();
                    this.setState({ isOpenForm: false });
                } else {
                    if (data.errors) {
                        let errors = {};
                        data.errors.map(error => {
                            errors[camelCase(error.key)] = error.errorMessage;
                        });

                        this.setState({ errors });
                    } else {
                        toast.error(data.message);
                    }
                }
            }).catch(axiosError);
        }
    }

    handleCancel = () => {
        this.setState({
            isOpenForm: false,
            performanceRecord: {
                employees: [],
                grade: null,
                comment: '',
                projects: [],
                statusId: null
            },
            errors: {}
        });
    }

    render() {
        let performanceRecord = this.state.performanceRecord;
        let isOpenForm = this.state.isOpenForm;
        let memberOptions = this.props.memberOptions;
        let performanceGradeOptions = this.props.performanceGradeOptions;
        let projectOptions = this.props.projectOptions;
        let errors = this.state.errors;
        let currentUser = this.props.currentUser;

        return (
            <div className="record record-your-mind">
                <div className="avatar-user">
                    <img src={currentUser.avatar} className="avatar" />
                    <div className="person-info">
                        <span className="role-member">{currentUser.nickName}</span>
                        <div className="submit-date">{this.state.currentDate.format(DateFormat)}</div>
                    </div>
                </div>
                <div className="comment">
                    {
                        !isOpenForm &&
                        <input className="form-control"
                            style={{ height: 50, marginBottom: 15 }}
                            placeholder="What's on your mind?"
                            onClick={this.handleOpenForm}
                        />
                    }
                    {
                        isOpenForm &&
                        <form className="form-filter form-performance create">
                            <FormGroup>
                                <ControlLabel className="required-label">{getResource('Member')}</ControlLabel>
                                <Select name="employees" isMulti closeMenuOnSelect={false} classNamePrefix="react-select"
                                    className={`react-select-container employee-field ${!isEmpty(errors) && errors.employee ? 'input-validation-error' : ''}`}
                                    placeholder="Select Employees"
                                    value={performanceRecord.employees} options={memberOptions}
                                    onChange={this.handleEmployeeChange} isClearable
                                />
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel className="required-label">{getResource('Comment')}</ControlLabel>
                                <TextareaFullscreenEditor
                                    className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                    name="comment"
                                    value={performanceRecord.comment}
                                    onChange={this.handleCommentChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <div className="grade">
                                    <ControlLabel>{getResource('Grade')}</ControlLabel>
                                    <Select name="grade" className="react-select-container grade-field" classNamePrefix="react-select"
                                        placeholder="Select Grade"
                                        value={performanceRecord.grade}
                                        options={performanceGradeOptions}
                                        isSearchable={false}
                                        isClearable
                                        onChange={this.handleGradeChange}
                                    />
                                </div>
                                <div className="project">
                                    <ControlLabel>{getResource('ProjectTag')}</ControlLabel>
                                    <Select name="projects" isMulti closeMenuOnSelect={false} placeholder="Projects"
                                        className="react-select-container project-field" classNamePrefix="react-select"
                                        value={performanceRecord.projects}
                                        options={projectOptions}
                                        onChange={this.handleProjectChange}
                                    />
                                </div>
                            </FormGroup>

                            <FormGroup>
                                <Button className="btn-cancel" onClick={this.handleCancel}>{getResource('Cancel')}</Button>
                                <Button className="btn-cancel" type="submit" name="saveAsDraft" onClick={this.handleSubmit}>{getResource('SaveAsDraft')}</Button>
                                <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmit}>{getResource('Submit')}</Button>
                            </FormGroup>
                        </form>
                    }
                </div>
            </div>
        );
    }
}