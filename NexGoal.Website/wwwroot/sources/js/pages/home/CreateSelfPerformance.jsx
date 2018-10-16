import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import avatar1 from '../../../styles/images/avatar.png';

export default class CreateSelfPerformance extends Component {
    static propTypes = {
        performanceGradeOptions: PropTypes.arrayOf(PropTypes.object),
        projectOptions: PropTypes.arrayOf(PropTypes.object),
        onSubmitSuccess: PropTypes.func
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
            errors: {}
        };
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

                    this.props.onSubmitSuccess(response.data);
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
        this.setState({ isOpenForm: false });
    }

    render() {
        let performanceRecord = this.state.performanceRecord;
        let isOpenForm = this.state.isOpenForm;
        let performanceGradeOptions = this.props.performanceGradeOptions;
        let projectOptions = this.props.projectOptions;
        let errors = this.state.errors;
        const avatar = `dist/styles/${avatar1}`;
        return (
            <Fragment>
                {
                    !isOpenForm &&
                    <div className="record record-your-mind">
                        <div className="avatar-user">
                            <img src={avatar} className="avatar" />
                            <div className="name">Raymond</div>
                            <div className="submit-date">Oct 10, 2018</div>
                        </div>
                        <div className="comment"><input className="form-control"
                            style={{ height: 50, marginBottom: 15 }}
                            placeholder="What's on your mind?"
                            onClick={this.handleOpenForm}
                        /></div>
                    </div>
                }

                {
                    isOpenForm &&
                    <div className="record record-your-mind">
                        <div className="avatar-user">
                            <img src={avatar} className="avatar" />
                            <div className="name">Raymond</div>
                            <div className="submit-date">Oct 10, 2018</div>
                        </div>
                        <div class="comment">
                            <form className="form-filter form-performance create">
                            {
                                !isEmpty(errors) &&
                                <ul className="list-message-error">
                                    {errors.comment && <li>{errors.comment}</li>}
                                </ul>
                            }
                                <FormGroup>
                                    <ControlLabel className="required-label">{getResource('Comment')}</ControlLabel>
                                    <FormControl componentClass="textarea" name="comment"
                                        className={`comment-field ${!isEmpty(errors) && errors.comment && 'input-validation-error'}`}
                                        value={performanceRecord.comment} onChange={this.handleCommentChange}
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
                        </div>
                    </div>
                    
                }
            </Fragment>
        );
    }
}