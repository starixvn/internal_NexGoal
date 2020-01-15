import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { Button, ControlLabel, Modal } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { isEmpty } from 'lodash';
import { stringify } from 'qs';
import moment from 'moment';
import isEqual from 'react-fast-compare';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PostRequestHeaders } from '../../constants';
import getResource from '../../resourceHelper';
import TextareaFullscreenEditor from '../../shared/components/textarea-fullscreen/index';
import PerformanceGradeInfo from '../../shared/components/PerformanceGradeInfo';
import { validateCreateEditSelfPerformance, axiosEditPerformanceResponse } from './common-functions';

export default class EditSelfPerformance extends Component {
    static propTypes = {
        performanceRecordId: PropTypes.number,
        performanceGradeOptions: PropTypes.arrayOf(PropTypes.object),
        projectOptions: PropTypes.arrayOf(PropTypes.object),
        currentUser: PropTypes.objectOf(PropTypes.object),
        showModal: PropTypes.bool,
        onCloseModal: PropTypes.func,
        onSubmitSuccess: PropTypes.func,
        isDraft: PropTypes.bool
    };

    constructor(props) {
        super(props);

        this.state = {
            performanceRecord: {},
            isSubmitting: false,
            errors: {},
            initialPerformanceRecord: {},
            showGradeInfo: false
        };
    }

    componentDidMount() {
        const url = this.props.isDraft ? `Performance/GetDraftById/${this.props.performanceRecordId}` : `Performance/GetById/${this.props.performanceRecordId}`;
        axios(resolveClientUrl(url))
            .then(response => {
                const performanceRecord = response.data;
                performanceRecord.grade = performanceRecord.grade ? {
                    value: performanceRecord.grade.gradeId,
                    label: `(${performanceRecord.grade.name}) ${performanceRecord.grade.description}`
                } : null;
                performanceRecord.projects = performanceRecord.projects.map(project => {
                    return { value: project.projectId, label: project.name };
                });

                this.setState({ performanceRecord: performanceRecord, initialPerformanceRecord: { ...performanceRecord } });
            }).catch(axiosError);

        window.addEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    componentWillUnmount() {
        window.removeEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    onLeavingPageEventListener = event => {
        let { performanceRecord, initialPerformanceRecord } = this.state;

        if (!isEqual(performanceRecord, initialPerformanceRecord)) {
            event.returnValue = ' ';
            return ' ';
        }

        return null;
    }

    handleCommentChange = event => {
        const errors = {};

        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { comment: event.target.value }),
            errors: errors
        });
    }

    handleGradeChange = selectedOption => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    }

    handleProjectChange = selectedOptions => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { projects: selectedOptions })
        });
    }

    handleSubmit = event => {
        event.preventDefault();

        if (this.state.isSubmitting) {
            return;
        }

        const isValid = validateCreateEditSelfPerformance(this);
        if (isValid) {
            this.setSubmitting(true);

            const record = this.state.performanceRecord;
            const performanceRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: record.performanceRecordId,
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment.trim(),
                isFromDraft: record.isDraft,
                projectIds: record.projects.map(project => project.value).join(',')
            });

            axios({
                url: resolveClientUrl('Self/Performance/Edit'),
                method: 'POST',
                data: performanceRecord,
                headers: PostRequestHeaders
            }).then(response => {
                axiosEditPerformanceResponse(this, toast, response, () => {
                    this.props.onSubmitSuccess(response.data.objectIds);
                });
            }).catch(axiosError).then(() => this.setSubmitting(false));
        }
    }

    handleSaveAsDraft = event => {
        event.preventDefault();

        if (this.state.isSubmitting) {
            return;
        }

        const isValid = validateCreateEditSelfPerformance(this);
        if (isValid) {
            this.setSubmitting(true);

            const record = this.state.performanceRecord;
            const performanceRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: record.performanceRecordId,
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment.trim(),
                projectIds: record.projects.map(project => project.value).join(',')
            });

            axios({
                url: resolveClientUrl('Self/Performance/EditDraft'),
                method: 'POST',
                data: performanceRecord,
                headers: PostRequestHeaders
            }).then(response => {
                axiosEditPerformanceResponse(this, toast, response, () => {
                    this.props.onSubmitDraftSuccess(record.performanceRecordId);
                });
            }).catch(axiosError).then(() => this.setSubmitting(false));
        }
    }

    setSubmitting = isSubmitting => this.setState({ isSubmitting })

    handleCloseGradeInfo = () => this.setState({ showGradeInfo: false })

    handleOpenGradeInfo = () => this.setState({ showGradeInfo: true })

    render() {
        const { performanceGradeOptions, projectOptions, currentUser } = this.props;
        const { performanceRecord, errors } = this.state;

        return (
            <Fragment>
                <Modal keyboard={false} backdrop="static"
                    show={this.props.showModal} onHide={this.props.onCloseModal}
                    dialogClassName="custom-modal modal-md form-performance edit"
                >
                    <Modal.Header>
                        <Modal.Title>{getResource('EditPerformanceCheckin')}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={currentUser.avatar} className="avatar" />
                                <div className="person-info">
                                    <span className="role-member">{currentUser.nickName}</span>
                                </div>
                            </div>
                            <div className="date-time self-checkin">
                                {
                                    performanceRecord.isDraft &&
                                    <span className="draft">draft</span>
                                }
                                <span className="submit-date">{moment().format(DateFormat)}</span>
                            </div>
                            <div className="comment">
                                <ControlLabel>{getResource('Comment')}</ControlLabel>
                                <TextareaFullscreenEditor
                                    className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                    name="comment"
                                    value={performanceRecord.comment}
                                    onChange={this.handleCommentChange}
                                />
                            </div>
                            <div className="grade">
                                <ControlLabel>{getResource('Grade')} <a className="grade-info" onClick={this.handleOpenGradeInfo} /></ControlLabel>
                                <Select name="grade" className="react-select-container grade-field" classNamePrefix="react-select"
                                    placeholder="Select Grade"
                                    value={performanceRecord.grade}
                                    options={performanceGradeOptions}
                                    isSearchable={false} isClearable
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
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                        {
                            performanceRecord.isDraft &&
                            <Button type="submit" className="btn-cancel" name="saveAsDraft"
                                onClick={this.handleSaveAsDraft} disabled={this.state.isSubmitting}
                            >
                                {getResource('SaveAsDraft')}
                            </Button>
                        }
                        <Button type="submit" bsStyle="primary" name="save"
                            onClick={this.handleSubmit} disabled={this.state.isSubmitting}
                        >
                            {getResource('Publish')}
                        </Button>
                    </Modal.Footer>
                </Modal>
                <PerformanceGradeInfo showModal={this.state.showGradeInfo} performanceGradeInfos={this.props.performanceGradeInfos} onCloseModal={this.handleCloseGradeInfo} />

            </Fragment>
        );
    }
}