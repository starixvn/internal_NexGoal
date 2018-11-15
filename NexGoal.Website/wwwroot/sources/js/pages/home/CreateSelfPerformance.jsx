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

export default class CreateSelfPerformance extends Component {
    static propTypes = {
        currentUser: PropTypes.object,
        performanceGradeOptions: PropTypes.arrayOf(PropTypes.object),
        projectOptions: PropTypes.arrayOf(PropTypes.object),
        onSubmitSuccess: PropTypes.func
    };

    static defaultProps = {
        currentUser: { nickName: '', avatar: '' },
        performanceGradeOptions: [],
        projectOptions: [],
        onSubmitSuccess: function () { }
    };

    constructor(props) {
        super(props);

        this.state = {
            isOpenForm: false,
            performanceRecord: {
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
    }

    handleGradeChange = (selectedOption) => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    }

    handleCommentChange = (event) => {
        let errors = {};

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

        if (!this.state.performanceRecord.comment || this.state.performanceRecord.comment.trim() === '') {
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

            let performanceRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                gradeId: this.state.performanceRecord.grade ? this.state.performanceRecord.grade.value : null,
                comment: this.state.performanceRecord.comment,
                projectIds: this.state.performanceRecord.projects.map((project) => { return project.value; }).join(','),
                statusId: statusId
            });

            axios({
                url: resolveClientUrl('Self/Performance/Create'),
                method: 'POST',
                data: performanceRecord,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success(getResource('CreatedSuccessfully'));

                    this.handleCancel();
                    this.props.onSubmitSuccess();
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