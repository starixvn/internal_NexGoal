import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { isEmpty } from 'lodash';
import { stringify } from 'qs';
import isEqual from 'react-fast-compare';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { ProbationTag, PostRequestHeaders } from '../../constants';
import getResource from '../../resourceHelper';
import TextareaFullscreenEditor from '../../shared/components/textarea-fullscreen/index';
import PerformanceGradeInfo from '../../shared/components/PerformanceGradeInfo';
import { validateCreateEditSelfPerformance, axiosCreatePerformanceResponse } from './common-functions';

export default class CreateSelfPerformance extends Component {
    static propTypes = {
        currentUser: PropTypes.objectOf(PropTypes.object),
        performanceGradeOptions: PropTypes.arrayOf(PropTypes.object),
        projectOptions: PropTypes.arrayOf(PropTypes.object),
        onSubmitSuccess: PropTypes.func
    };

    static defaultProps = {
        currentUser: { nickName: '', avatar: '' },
        performanceGradeOptions: [],
        projectOptions: [],
        onSubmitSuccess: function () {
            // default: do nothing
        }
    };

    constructor(props) {
        super(props);

        this.state = {
            isOpenForm: false,
            performanceRecord: {
                grade: null,
                comment: '',
                projects: props.currentUser.isProbation ? [ProbationTag] : [],
                statusId: null
            },
            initialPerformanceRecord: {
                grade: null,
                comment: '',
                projects: props.currentUser.isProbation ? [ProbationTag] : [],
                statusId: null
            },
            errors: {},
            isSubmitting: false,
            showGradeInfo: false
        };

        this.textareaEditor = React.createRef();
    }

    componentDidMount() {
        window.addEventListener('beforeunload', this.onCreateSelfPerformanceLeavingPageEventListener);
    }

    shouldComponentUpdate(nextProps, nextState) {
        return !isEqual(this.state, nextState) || !isEqual(this.props, nextProps);
    }

    onCreateSelfPerformanceLeavingPageEventListener = event => {
        if (!isEqual(this.state.performanceRecord, this.state.initialPerformanceRecord)) {
            event.returnValue = ' ';
            return ' ';
        }

        return null;
    };

    handleGradeChange = selectedOption => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    };

    handleOpenForm = () => {
        this.setState({ isOpenForm: true }, () => {
            this.textareaEditor.current.textareaEditor.current.focus();
        });
    };

    handleCommentChange = event => {
        const errors = {};

        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { comment: event.target.value }),
            errors: errors
        });
    };

    handleProjectChange = selectedOptions => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { projects: selectedOptions })
        });
    };

    getAxiosData = () => {
        const performanceRecord = this.state.performanceRecord;
        return stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            gradeId: performanceRecord.grade ? performanceRecord.grade.value : null,
            comment: performanceRecord.comment.trim(),
            projectIds: performanceRecord.projects.map(project => project.value).join(',')
        });
    };

    handleSaveAsDraft = event => {
        event.preventDefault();

        if (this.state.isSubmitting) {
            return;
        }

        const isValid = validateCreateEditSelfPerformance(this);
        if (isValid) {
            this.setSubmitting(true);

            axios({
                url: resolveClientUrl('Self/Performance/CreateDraft'),
                method: 'POST',
                data: this.getAxiosData(),
                headers: PostRequestHeaders
            }).then(response => {
                axiosCreatePerformanceResponse(this, toast, response, () => {
                    this.handleCancel();
                    this.props.onSubmitDraftSuccess(response.data.objectIds);
                });
            }).catch(axiosError).then(() => this.setSubmitting(false));
        }
    };

    handleSubmit = event => {
        event.preventDefault();

        if (this.state.isSubmitting) {
            return;
        }

        const isValid = validateCreateEditSelfPerformance(this);
        if (isValid) {
            this.setSubmitting(true);

            axios({
                url: resolveClientUrl('Self/Performance/Create'),
                method: 'POST',
                data: this.getAxiosData(),
                headers: PostRequestHeaders
            }).then(response => {
                axiosCreatePerformanceResponse(this, toast, response, () => {
                    this.handleCancel();
                    this.props.onSubmitSuccess(response.data.objectIds);
                });
            }).catch(axiosError).then(() => this.setSubmitting(false));
        }
    };

    handleCancel = () => {
        this.setState({
            isOpenForm: false,
            performanceRecord: {
                grade: null,
                comment: '',
                projects: this.props.currentUser.isProbation ? [ProbationTag] : [],
                statusId: null
            },
            errors: {}
        });
    };

    handleOpenGradeInfo = () => this.setState({ showGradeInfo: true });

    handleCloseGradeInfo = () => this.setState({ showGradeInfo: false });

    setSubmitting = isSubmitting => this.setState({ isSubmitting });

    render() {
        const { performanceRecord, isOpenForm, errors } = this.state;
        const { performanceGradeOptions, projectOptions, currentUser } = this.props;

        return (
            <div className="record record-your-mind">
                <div className="avatar-user">
                    <img src={currentUser.avatar} className="avatar" />
                    <div className="person-info">
                        <span className="role-member">{currentUser.nickName}</span>
                    </div>
                </div>
                <div className="comment">
                    {
                        !isOpenForm &&
                        <input className="form-control"
                            style={{ height: 50 }}
                            placeholder="Any achievements you are proud of?"
                            onClick={this.handleOpenForm}
                        />
                    }
                    {
                        isOpenForm &&
                        <form className="form-filter form-performance create">
                            <FormGroup>
                                <ControlLabel>{getResource('Comment')}</ControlLabel>
                                <TextareaFullscreenEditor ref={this.textareaEditor}
                                    name="comment"
                                    className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                    onChange={this.handleCommentChange}
                                    value={performanceRecord.comment}
                                />
                            </FormGroup>
                            <FormGroup>
                                <div className="grade">
                                    <ControlLabel>{getResource('Grade')} <a className="grade-info" onClick={this.handleOpenGradeInfo} /></ControlLabel>
                                    <Select name="grade" className="react-select-container grade-field"
                                        classNamePrefix="react-select"
                                        placeholder="Select Grade"
                                        options={performanceGradeOptions}
                                        value={performanceRecord.grade}
                                        onChange={this.handleGradeChange}
                                        isSearchable={false}
                                        isClearable
                                    />
                                </div>
                                <div className="project">
                                    <ControlLabel>{getResource('ProjectTag')}</ControlLabel>
                                    <Select name="projects" closeMenuOnSelect={false} placeholder="Projects"
                                        className="react-select-container project-field"
                                        classNamePrefix="react-select"
                                        options={projectOptions}
                                        value={performanceRecord.projects}
                                        onChange={this.handleProjectChange}
                                        isSearchable={false}
                                        isMulti
                                    />
                                </div>
                            </FormGroup>

                            <FormGroup className="text-right">
                                <Button className="btn-cancel" onClick={this.handleCancel}>{getResource('Cancel')}</Button>
                                <Button className="btn-cancel" type="submit" name="saveAsDraft" onClick={this.handleSaveAsDraft} disabled={this.state.isSubmitting}>
                                    {getResource('SaveAsDraft')}
                                </Button>
                                <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmit} disabled={this.state.isSubmitting}>{getResource('Publish')}</Button>
                            </FormGroup>
                        </form>
                    }
                </div>
                <PerformanceGradeInfo showModal={this.state.showGradeInfo} performanceGradeInfos={this.props.performanceGradeInfos} onCloseModal={this.handleCloseGradeInfo} />
            </div>
        );
    }
}